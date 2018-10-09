using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AppDb.Interfaces;
using AppDb.Models.Entities;
using AppDb.Services;
using AppModel.Enumerations;
using AppModel.Enumerations.Order;
using AppModel.Exceptions;
using Main.Interfaces.Services;
using Main.Interfaces.Services.Businesses;
using Main.Models;
using Main.ViewModels.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Services;
using Shared.Models;
using Shared.Resources;
using Shared.Services;

namespace Main.Services.Businesses
{
    public class UserService : IUserService
    {
        #region Properties

        private readonly IEncryptionService _encryptionService;

        private readonly IIdentityService _identityService;

        private readonly HttpContext _httpContext;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IExternalAuthenticationService _externalAuthenticationService;

        private readonly ITimeService _timeService;

        private readonly ApplicationSetting _applicationSettings;

        private readonly IRelationalDbService _relationalDbService;

        #endregion

        #region Constructors

        public UserService(IEncryptionService encryptionService, IIdentityService identityService, 
            IUnitOfWork unitOfWork, 
            IHttpContextAccessor httpContextAccessor, 
            IExternalAuthenticationService externalAuthenticationService, 
            ITimeService timeService, 
            IRelationalDbService relationalDbService,
            ApplicationSetting applicationSettings)
        {
            _encryptionService = encryptionService;
            _identityService = identityService;
            _unitOfWork = unitOfWork;
            _httpContext = httpContextAccessor.HttpContext;
            _externalAuthenticationService = externalAuthenticationService;
            _timeService = timeService;
            _applicationSettings = applicationSettings;
            _relationalDbService = relationalDbService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<User> LoginAsync(LoginViewModel model, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Hash the password first.
            var hashedPassword = _encryptionService.Md5Hash(model.Password);

            // Search for account which is active and information is correct.
            var users = _unitOfWork.Accounts.Search();
            users = users.Where(x =>
                x.Email.Equals(model.Email, StringComparison.InvariantCultureIgnoreCase) &&
                x.Password.Equals(hashedPassword, StringComparison.InvariantCultureIgnoreCase) && x.Type == UserKind.Basic);

            // Find the first account in database.
            var user = await users.FirstOrDefaultAsync(cancellationToken);
            if (user == null)
                throw new ApiException(HttpMessages.AccountIsNotFound, HttpStatusCode.NotFound);

            switch (user.Status)
            {
                case UserStatus.Pending:
                    throw new ApiException(HttpMessages.AccountIsPending, HttpStatusCode.Forbidden);
                case UserStatus.Disabled:
                    throw new ApiException(HttpMessages.AccountIsDisabled, HttpStatusCode.Forbidden);
            }

            return user;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<User> GoogleLoginAsync(GoogleLoginViewModel model, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Get the profile information.
            var profile = await _externalAuthenticationService.GetGoogleBasicProfileAsync(model.IdToken);
            if (profile == null)
                throw new ApiException(HttpMessages.GoogleCodeIsInvalid, HttpStatusCode.Forbidden);

            // Find accounts by searching for email address.
            var users = _unitOfWork.Accounts.Search();
            users = users.Where(x => x.Email.Equals(profile.Email));

            // Get the first matched account.
            var user = await users.FirstOrDefaultAsync(cancellationToken);

            // Account is available in the system. Check its status.
            if (user != null)
            {
                // Prevent account from logging into system because it is pending.
                if (user.Status == UserStatus.Pending)
                    throw new ApiException(HttpMessages.AccountIsPending, HttpStatusCode.Forbidden);

                // Prevent account from logging into system because it is deleted.
                if (user.Status == UserStatus.Disabled)
                    throw new ApiException(HttpMessages.AccountIsDisabled, HttpStatusCode.Forbidden);
            }
            else
            {
                // Initialize account instance.
                user = new User();

#if USE_IN_MEMORY
                user.Id = _unitOfWork.Accounts.Search().OrderByDescending(x => x.Id).Select(x => x.Id)
                                 .FirstOrDefault() + 1;
#endif
                user.Email = profile.Email;
                user.Nickname = profile.Name;
                user.Role = UserRole.User;
                user.Photo = profile.Picture;
                user.JoinedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);
                user.Type = UserKind.Google;
                user.Status = UserStatus.Available;

                // Add account to database.
                _unitOfWork.Accounts.Insert(user);
                await _unitOfWork.CommitAsync(cancellationToken);
            }
            return user;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<User> FacebookLoginAsync(FacebookLoginViewModel model, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Find token information.
            var tokenInfo = await _externalAuthenticationService.GetFacebookTokenInfoAsync(model.AccessToken);
            if (tokenInfo == null || string.IsNullOrWhiteSpace(tokenInfo.AccessToken))
                throw new ApiException(HttpMessages.FacebookCodeIsInvalid, HttpStatusCode.Forbidden);

            // Get the profile information.
            var profile = await _externalAuthenticationService.GetFacebookBasicProfileAsync(tokenInfo.AccessToken);
            if (profile == null)
                throw new ApiException(HttpMessages.GoogleCodeIsInvalid, HttpStatusCode.Forbidden);


            // Find accounts by searching for email address.
            var accounts = _unitOfWork.Accounts.Search();
            accounts = accounts.Where(x => x.Email.Equals(profile.Email));

            // Get the first matched account.
            var account = await accounts.FirstOrDefaultAsync(cancellationToken);

            // Account is available in the system. Check its status.
            if (account != null)
            {
                // Prevent account from logging into system because it is pending.
                if (account.Status == UserStatus.Pending)
                    throw new ApiException(HttpMessages.AccountIsPending, HttpStatusCode.Forbidden);

                // Prevent account from logging into system because it is deleted.
                if (account.Status == UserStatus.Disabled)
                    throw new ApiException(HttpMessages.AccountIsPending, HttpStatusCode.Forbidden);
            }
            else
            {
                // Initialize account instance.
                account = new User();
                account.Email = profile.Email;
                account.Nickname = profile.FullName;
                account.Role = UserRole.User;
                account.JoinedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);
                account.Type = UserKind.Facebook;

                // Add account to database.
                _unitOfWork.Accounts.Insert(account);
                await _unitOfWork.CommitAsync(cancellationToken);
            }

            return account;
        }

        /// <summary>
        /// Register a basic account.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<User> BasicRegisterAsync(RegisterAccountViewModel model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Search for duplicated accounts.
            var users = _unitOfWork.Accounts.Search();
            users = users.Where(
                x => x.Email.Equals(model.Email, StringComparison.InvariantCultureIgnoreCase));

            // Find the first matched account.
            var user = await users.FirstOrDefaultAsync(cancellationToken);

            // Account exists in system.
            if (user != null)
                throw new ApiException(HttpMessages.AccountIsInUse, HttpStatusCode.Conflict);

            var transaction = _unitOfWork.BeginTransactionScope();

            // Initiate account with specific information.
            user = new User();
            user.Email = model.Email;
            user.Password = _encryptionService.Md5Hash(model.Password);
            user.Nickname = model.Nickname;

            // Add account into database.
            _unitOfWork.Accounts.Insert(user);

            var activationToken = new ActivationToken();
            activationToken.OwnerId = user.Id;
            activationToken.Code = Guid.NewGuid().ToString("D");
            activationToken.IssuedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);
            activationToken.ExpiredTime = activationToken.IssuedTime + 3600;
            _unitOfWork.ActivationTokens.Insert(activationToken);

            // Commit the transaction.
            _unitOfWork.Commit();
            transaction.Commit();

            return user;
        }

        /// <summary>
        /// Search users using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<SearchResult<IList<User>>> SearchUsersAsync(SearchUserViewModel condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            var users = GetUsers(condition);

            // Sort by properties.
            if (condition.Sort != null)
                users =
                    _relationalDbService.Sort(users, condition.Sort.Direction,
                        condition.Sort.Property);
            else
                users = _relationalDbService.Sort(users, SortDirection.Decending,
                    AccountSort.JoinedTime);

            // Result initialization.
            var loadUsersResult = new SearchResult<IList<User>>();
            loadUsersResult.Total = await users.CountAsync(cancellationToken);
            loadUsersResult.Records = await _relationalDbService.Paginate(users, condition.Pagination).ToListAsync(cancellationToken);
            return loadUsersResult;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<User> SearchUserAsync(SearchUserViewModel condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetUsers(condition).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Request for password change token.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<ForgotPasswordResultModel> RequestPasswordResetAsync(ForgotPasswordViewModel model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var loadUserCondition = new SearchUserViewModel();
            loadUserCondition.Emails = new HashSet<string> { model.Email };
            loadUserCondition.Statuses = new HashSet<UserStatus> { UserStatus.Available };

            var user = await GetUsers(loadUserCondition).FirstOrDefaultAsync(cancellationToken);
            if (user == null)
                throw new ApiException(HttpMessages.AccountIsNotFound, HttpStatusCode.NotFound);

            // Find current system time.
            var systemTime = DateTime.UtcNow;
            var expiration = systemTime.AddSeconds(_applicationSettings.PasswordResetTokenLifeTime);

            // Initiate token.
            var token = new AccessToken();
            token.OwnerId = user.Id;
            //token.Type = TokenType.AccountReactiveCode;
            token.Code = Guid.NewGuid().ToString("D");
            token.IssuedTime = _timeService.DateTimeUtcToUnix(systemTime);
            token.ExpiredTime = _timeService.DateTimeUtcToUnix(expiration);

            // Save token into database.
            _unitOfWork.AccessTokens.Insert(token);
            await _unitOfWork.CommitAsync(cancellationToken);

            var forgotPasswordResult = new ForgotPasswordResultModel();
            forgotPasswordResult.Email = user.Email;
            forgotPasswordResult.AccessToken = token.Code;
            return forgotPasswordResult;
        }

        /// <summary>
        /// Get users using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected virtual IQueryable<User> GetUsers(SearchUserViewModel condition)
        {
            // Get all users.
            var users = _unitOfWork.Accounts.Search();

            // Get requester profile.
            var profile = _identityService.GetProfile(_httpContext);
            
            // Id have been defined.
            var ids = condition.Ids;
            if (ids != null && ids.Count > 0)
            {
                ids = ids.Where(x => x > 0).ToHashSet();
                if (ids != null && ids.Count > 0)
                    users = users.Where(x => condition.Ids.Contains(x.Id));
            }

            // Email have been defined.
            var emails = condition.Emails;
            if (emails != null && emails.Count > 0)
            {
                emails = emails.Where(x => !string.IsNullOrEmpty(x)).ToHashSet();
                if (emails != null && emails.Count > 0)
                    users = users.Where(x => condition.Emails.Any(y => x.Email.Contains(y)));
            }

            // Search conditions which are based on roles.

            if (profile != null && profile.Role == UserRole.Admin)
            {
                // Statuses have been defined.
                var statuses = condition.Statuses;
                if (statuses != null && statuses.Count > 0)
                {
                    statuses =
                        statuses.Where(x => Enum.IsDefined(typeof(UserStatus), x)).ToHashSet();
                    if (statuses.Count > 0)
                        users = users.Where(x => condition.Statuses.Contains(x.Status));
                }

                // Roles have been defined.
                var roles = condition.Roles;
                if (condition.Roles != null && condition.Roles.Count > 0)
                {
                    roles =
                       roles.Where(x => Enum.IsDefined(typeof(UserRole), x)).ToHashSet();
                    if (roles.Count > 0)
                        users = users.Where(x => roles.Contains(x.Role));
                }
            }
            return users;
        }

        #endregion
    }
}