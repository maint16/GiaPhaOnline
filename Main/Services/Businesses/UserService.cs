using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AppDb.Interfaces;
using AppDb.Models.Entities;
using AppModel.Enumerations;
using AppModel.Enumerations.Order;
using AppModel.Exceptions;
using Main.Constants;
using Main.Interfaces.Services;
using Main.Interfaces.Services.Businesses;
using Main.Models;
using Main.Models.Jwt;
using Main.ViewModels.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Interfaces.Services;
using Shared.Models;
using Shared.Resources;
using SkiaSharp;
using VgySdk.Interfaces;
using VgySdk.Models;

namespace Main.Services.Businesses
{
    public class UserService : IUserService
    {
        #region Constructors

        public UserService(IEncryptionService encryptionService,
            IUnitOfWork unitOfWork,
            IExternalAuthenticationService externalAuthenticationService,
            IValueCacheService<int, User> profileCacheService,
            ITimeService timeService,
            IRelationalDbService relationalDbService,
            IVgyService vgyService,
            IOptions<ApplicationSetting> applicationSettingOptions,
            IOptions<JwtConfiguration> jwtConfigurationOptions)
        {
            _encryptionService = encryptionService;
            _unitOfWork = unitOfWork;
            _externalAuthenticationService = externalAuthenticationService;
            _timeService = timeService;
            _applicationSettings = applicationSettingOptions.Value;
            _relationalDbService = relationalDbService;
            _jwtConfiguration = jwtConfigurationOptions.Value;
            _profileCacheService = profileCacheService;
            _vgyService = vgyService;
        }

        #endregion

        #region Properties

        private readonly IEncryptionService _encryptionService;
        

        private readonly IUnitOfWork _unitOfWork;

        private readonly IExternalAuthenticationService _externalAuthenticationService;

        private readonly ITimeService _timeService;

        private readonly ApplicationSetting _applicationSettings;

        private readonly IRelationalDbService _relationalDbService;

        private readonly JwtConfiguration _jwtConfiguration;

        /// <summary>
        ///     Service which is for handling profile caching.
        /// </summary>
        private readonly IValueCacheService<int, User> _profileCacheService;

        private readonly IVgyService _vgyService;

        #endregion

        #region Methods

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<User> LoginAsync(LoginViewModel model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Hash the password first.
            var hashedPassword = _encryptionService.Md5Hash(model.Password);

            // Search for account which is active and information is correct.
            var users = _unitOfWork.Accounts.Search();
            users = users.Where(x =>
                x.Email.Equals(model.Email, StringComparison.InvariantCultureIgnoreCase) &&
                x.Password.Equals(hashedPassword, StringComparison.InvariantCultureIgnoreCase) &&
                x.Type == UserKind.Basic);

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
        ///     <inheritdoc />
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<User> GoogleLoginAsync(GoogleLoginViewModel model,
            CancellationToken cancellationToken = default(CancellationToken))
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
        ///     <inheritdoc />
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<User> FacebookLoginAsync(FacebookLoginViewModel model,
            CancellationToken cancellationToken = default(CancellationToken))
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
        ///     Register a basic account.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<BasicRegisterResultModel> BasicRegisterAsync(RegisterAccountViewModel model,
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

            var basicRegisterResult = new BasicRegisterResultModel(user.Email, activationToken.Code, user.Nickname);
            return basicRegisterResult;
        }

        /// <summary>
        ///     Search users using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<SearchResult<IList<User>>> SearchUsersAsync(SearchUserViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken))
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
            loadUsersResult.Records = await _relationalDbService.Paginate(users, condition.Pagination)
                .ToListAsync(cancellationToken);
            return loadUsersResult;
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<User> SearchUserAsync(SearchUserViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetUsers(condition).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        ///     Request for password change token.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<ForgotPasswordResultModel> RequestPasswordResetAsync(ForgotPasswordViewModel model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var loadUserCondition = new SearchUserViewModel();
            loadUserCondition.Emails = new HashSet<string> {model.Email};
            loadUserCondition.Statuses = new HashSet<UserStatus> {UserStatus.Available};

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
            forgotPasswordResult.Token = token.Code;
            return forgotPasswordResult;
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="photo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<User> UploadUserProfileImageAsync(int id, SKBitmap photo,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Find user.
            var user = await _unitOfWork.Accounts.FirstOrDefaultAsync(
                x => x.Id == id && x.Status == UserStatus.Available, cancellationToken);
            if (user == null)
                throw new ApiException(HttpMessages.AccountIsNotFound, HttpStatusCode.NotFound);

            // Resize image to 512x512 size.
            var resizedSkBitmap = photo.Resize(new SKImageInfo(512, 512), SKBitmapResizeMethod.Lanczos3);

            // Initialize file name.
            var fileName = $"{Guid.NewGuid():D}.png";

            using (var skImage = SKImage.FromBitmap(resizedSkBitmap))
            using (var skData = skImage.Encode(SKEncodedImageFormat.Png, 100))
            using (var memoryStream = new MemoryStream())
            {
                skData.SaveTo(memoryStream);
                var vgySuccessRespone = await _vgyService.UploadAsync<VgySuccessResponse>(memoryStream.ToArray(),
                    "image/png", fileName,
                    CancellationToken.None);

                // Response is empty.
                if (vgySuccessRespone == null || vgySuccessRespone.IsError)
                    throw new ApiException(HttpMessages.ImageIsInvalid, HttpStatusCode.Forbidden);

                user.Photo = vgySuccessRespone.ImageUrl;
            }

            // Save changes into database.
            await _unitOfWork.CommitAsync(cancellationToken);
            return user;
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<User> ChangePasswordAsync(int id, ChangePasswordViewModel model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            #region Find user

            var loadUserCondition = new SearchUserViewModel();
            loadUserCondition.Ids = new HashSet<int> {id};
            loadUserCondition.Statuses = new HashSet<UserStatus> {UserStatus.Available};

            var user = await SearchUserAsync(loadUserCondition, cancellationToken);

            if (user == null)
                throw new ApiException(HttpMessages.AccountIsNotFound, HttpStatusCode.NotFound);

            #endregion

            // Hash the curent password.
            var hashedCurrentPassword = _encryptionService.Md5Hash(model.OriginalPassword);
            if (!user.Password.Equals(hashedCurrentPassword, StringComparison.CurrentCultureIgnoreCase))
                throw new ApiException(HttpMessages.CurrentPasswordIsInvalid, HttpStatusCode.Forbidden);

            user.Password = _encryptionService.Md5Hash(model.Password);

            await _unitOfWork.CommitAsync(cancellationToken);
            return user;
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<User> ChangeUserStatus(int id, ChangeUserStatusViewModel model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            #region Find user

            var loadUserCondition = new SearchUserViewModel();
            loadUserCondition.Ids = new HashSet<int> {id};

            var user = await SearchUserAsync(loadUserCondition, cancellationToken);
            if (user == null)
                throw new ApiException(HttpMessages.AccountIsNotFound, HttpStatusCode.NotFound);

            #endregion

            #region Update user status

            // Whether information has been changed or not.
            var bHasInformationChanged = false;

            // Status has been defined.
            if (model.Status != user.Status)
                if (model.Status == UserStatus.Pending)
                {
                    user.Status = UserStatus.Disabled;
                    bHasInformationChanged = true;
                }
                else
                {
                    user.Status = model.Status;
                    bHasInformationChanged = true;
                }

            //todo: Reason

            // Information has been changed.
            if (!bHasInformationChanged)
                throw new NotModifiedException();

            #endregion

            await _unitOfWork.CommitAsync(cancellationToken);
            return user;
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<GenerateUserActivationTokenResult> RequestUserActivationTokenAsync(
            RequestUserActivationCodeViewModel model, CancellationToken cancellationToken = default(CancellationToken))
        {
            #region Search for user

            var user = await _unitOfWork.Accounts.FirstOrDefaultAsync(
                x => x.Email.Equals(model.Email, StringComparison.InvariantCultureIgnoreCase) &&
                     x.Status == UserStatus.Pending && x.Type == UserKind.Basic, cancellationToken);
            if (user == null)
                throw new ApiException(HttpMessages.AccountIsNotFound, HttpStatusCode.NotFound);

            #endregion

            #region Token generation

            // Find the existing token.
            var activationTokens = _unitOfWork.ActivationTokens.Search(x => x.OwnerId == user.Id);
            _unitOfWork.ActivationTokens.Remove(activationTokens);

            // Find current system time.
            var systemTime = DateTime.UtcNow;
            var expiration = systemTime.AddSeconds(_applicationSettings.PasswordResetTokenLifeTime);

            var activationToken = new ActivationToken();
            activationToken.Code = Guid.NewGuid().ToString("D");
            activationToken.OwnerId = user.Id;
            activationToken.IssuedTime = _timeService.DateTimeUtcToUnix(systemTime);
            activationToken.ExpiredTime = _timeService.DateTimeUtcToUnix(expiration);

            // Add token into database
            _unitOfWork.ActivationTokens.Insert(activationToken);

            // Save changes asychronously.
            await _unitOfWork.CommitAsync(cancellationToken);

            #endregion

            var addActivationTokenResult = new GenerateUserActivationTokenResult();
            return addActivationTokenResult;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<SubmitPasswordResetResultModel> SubmitPasswordResetAsync(SubmitPasswordResetViewModel model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            #region Search users & activation code.

            // Find active accounts.
            var user = await _unitOfWork.Accounts.FirstOrDefaultAsync(x => x.Email.Equals(model.Email, StringComparison.InvariantCultureIgnoreCase) && x.Status == UserStatus.Available, cancellationToken);
            if (user == null)
                throw new ApiException(HttpMessages.AccountIsNotFound, HttpStatusCode.NotFound);

            // Find active token.
            var epochSystemTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);
            var activationTokens = _unitOfWork.ActivationTokens.Search(x => x.ExpiredTime < epochSystemTime && x.OwnerId == user.Id && x.Code.Equals(model.ActivationCode));

            if (!await activationTokens.AnyAsync(cancellationToken))
                throw new ApiException(HttpMessages.ActivationCodeNotFound, HttpStatusCode.NotFound);
            
            #endregion

            #region Information change

            // Hash the password.
            var hashedPassword = _encryptionService.Md5Hash(model.Password);

            // Delete all found tokens.
            _unitOfWork.ActivationTokens.Remove(activationTokens);
            user.Password = hashedPassword;

            // Commit changes.
            await _unitOfWork.CommitAsync(cancellationToken);

            #endregion
            
            var submitPasswordResetResult = new SubmitPasswordResetResultModel();
            return submitPasswordResetResult;
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public virtual JsonWebTokenViewModel GenerateUserAccessToken(User user)
        {
            // Find current time on the system.
            var systemTime = DateTime.Now;
            var jwtExpiration = systemTime.AddSeconds(_jwtConfiguration.LifeTime);

            // Claims initalization.
            var claims = new List<Claim>();
            claims.Add(new Claim(JwtRegisteredClaimNames.Aud, _jwtConfiguration.Audience));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iss, _jwtConfiguration.Issuer));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(nameof(user.Nickname), user.Nickname));
            claims.Add(new Claim(nameof(user.Id), user.Id.ToString()));

            // Write a security token.
            var jwtSecurityToken = new JwtSecurityToken(_jwtConfiguration.Issuer, _jwtConfiguration.Audience, claims,
                null, jwtExpiration, _jwtConfiguration.SigningCredentials);

            // Initiate token handler which is for generating token code.
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);

            // Initialize jwt response.
            var jwt = new JsonWebTokenViewModel();
            jwt.AccessToken = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);
            jwt.LifeTime = _jwtConfiguration.LifeTime;
            jwt.Expiration = _timeService.DateTimeUtcToUnix(jwtExpiration);

            _profileCacheService.Add(user.Id, user, LifeTimeConstant.JwtLifeTime);
            return jwt;
        }

        /// <summary>
        ///     Get users using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected virtual IQueryable<User> GetUsers(SearchUserViewModel condition)
        {
            // Get all users.
            var users = _unitOfWork.Accounts.Search();

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

            return users;
        }

        #endregion
    }
}