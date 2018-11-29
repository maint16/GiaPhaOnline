using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ClientShared.Enumerations;
using ClientShared.Enumerations.Order;
using ClientShared.Models;
using MainBusiness.Interfaces;
using MainBusiness.Interfaces.Domains;
using MainBusiness.Models.Users;
using MainDb.Interfaces;
using MainDb.Models.Entities;
using MainModel.Models;
using MainShared.Resources;
using MainShared.ViewModels.Jwt;
using MainShared.ViewModels.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServiceShared.Exceptions;
using ServiceShared.Interfaces.Services;
using ServiceShared.Models;
using SkiaSharp;
using VgySdk.Interfaces;
using VgySdk.Models;
using SortDirection = ClientShared.Enumerations.SortDirection;

namespace MainBusiness.Domain
{
    public class UserDomain : IUserDomain
    {
        #region Constructors

        public UserDomain(IBaseEncryptionService encryptionService,
            IAppUnitOfWork unitOfWork,
            IExternalAuthenticationService externalAuthenticationService,
            IBaseKeyValueCacheService<int, User> profileCacheService,
            IBaseTimeService baseTimeService,
            IBaseRelationalDbService relationalDbService,
            IAppProfileService profileService,
            IVgyService vgyService,
            IOptions<ApplicationSetting> applicationSettingOptions,
            IOptions<AppJwtModel> appJwt)
        {
            _encryptionService = encryptionService;
            _unitOfWork = unitOfWork;
            _externalAuthenticationService = externalAuthenticationService;
            _baseTimeService = baseTimeService;
            _applicationSettings = applicationSettingOptions.Value;
            _relationalDbService = relationalDbService;
            _appJwt = appJwt.Value;
            _profileCacheService = profileCacheService;
            _vgyService = vgyService;
            _profileService = profileService;
        }

        #endregion

        #region Properties

        private readonly IBaseEncryptionService _encryptionService;

        private readonly IAppUnitOfWork _unitOfWork;

        private readonly IExternalAuthenticationService _externalAuthenticationService;

        private readonly IBaseTimeService _baseTimeService;

        private readonly ApplicationSetting _applicationSettings;

        private readonly IBaseRelationalDbService _relationalDbService;

        private readonly AppJwtModel _appJwt;

        private readonly IAppProfileService _profileService;

        /// <summary>
        ///     Service which is for handling profile caching.
        /// </summary>
        private readonly IBaseKeyValueCacheService<int, User> _profileCacheService;

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
                user.JoinedTime = _baseTimeService.DateTimeUtcToUnix(DateTime.UtcNow);
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
            //// Find token information.
            //var tokenInfo = await _externalAuthenticationService.GetFacebookTokenInfoAsync(model.AccessToken);
            //if (tokenInfo == null || string.IsNullOrWhiteSpace(tokenInfo.AccessToken))
            //    throw new ApiException(HttpMessages.FacebookCodeIsInvalid, HttpStatusCode.Forbidden);

            // Get the profile information.
            var profile = await _externalAuthenticationService.GetFacebookBasicProfileAsync(model.AccessToken);
            if (profile == null)
                throw new ApiException(HttpMessages.GoogleCodeIsInvalid, HttpStatusCode.Forbidden);


            // Find accounts by searching for email address.
            var accounts = _unitOfWork.Accounts.Search();
            accounts = accounts.Where(x => x.Email.Equals(profile.Email));

            // Get the first matched account.
            var user = await accounts.FirstOrDefaultAsync(cancellationToken);

            // Account is available in the system. Check its status.
            if (user != null)
            {
                // Prevent account from logging into system because it is pending.
                if (user.Status == UserStatus.Pending)
                    throw new ApiException(HttpMessages.AccountIsPending, HttpStatusCode.Forbidden);

                // Prevent account from logging into system because it is deleted.
                if (user.Status == UserStatus.Disabled)
                    throw new ApiException(HttpMessages.AccountIsPending, HttpStatusCode.Forbidden);
            }
            else
            {
                // Initialize account instance.
                user = new User();
                user.Email = profile.Email;
                user.Nickname = profile.FullName;
                user.Role = UserRole.User;
                user.JoinedTime = _baseTimeService.DateTimeUtcToUnix(DateTime.UtcNow);
                user.Type = UserKind.Facebook;
                user.Status = UserStatus.Available;

                // Add account to database.
                _unitOfWork.Accounts.Insert(user);
                await _unitOfWork.CommitAsync(cancellationToken);
            }

            return user;
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
                    UserSort.JoinedTime);

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
        ///     Add/edit user signature.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<User> AddUserSignatureAsync(AddUserSignatureViewModel model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var profile = _profileService.GetProfile();
            if (profile == null)
                throw new Exception("No profile is found");

            var userId = model.UserId;
            if (profile.Role == UserRole.User || model.UserId == null)
                userId = profile.Id;

            var users = _unitOfWork.Accounts.Search(x => x.Id == userId && x.Status == UserStatus.Available);
            var user = await users.FirstOrDefaultAsync(cancellationToken);

            if (user == null)
                throw new ApiException(HttpStatusCode.NotFound, HttpMessages.AccountIsNotFound);

            user.Signature = model.Signature;
            await _unitOfWork.CommitAsync(cancellationToken);
            return user;
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public JwtViewModel GenerateJwt(User user)
        {
            // Find current time on the system.
            var systemTime = DateTime.Now;
            var jwtExpiration = systemTime.AddSeconds(_appJwt.LifeTime);

            // Claims initalization.
            var claims = new List<Claim>();
            claims.Add(new Claim(JwtRegisteredClaimNames.Aud, _appJwt.Audience));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iss, _appJwt.Issuer));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(nameof(user.Nickname), user.Nickname));
            claims.Add(new Claim(nameof(user.Id), user.Id.ToString()));

            // Write a security token.
            var jwtSecurityToken = new JwtSecurityToken(_appJwt.Issuer, _appJwt.Audience, claims,
                null, jwtExpiration, _appJwt.SigningCredentials);

            // Initiate token handler which is for generating token code.
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);

            // Initialize jwt response.
            var jwt = new JwtViewModel();
            jwt.AccessToken = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);
            jwt.LifeTime = _appJwt.LifeTime;
            jwt.Expiration = _baseTimeService.DateTimeUtcToUnix(jwtExpiration);

            //_profileCacheService.Add(user.Id, user, LifeTimeConstant.JwtLifeTime);
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