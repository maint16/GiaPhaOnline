using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AuthenticationBusiness.Interfaces;
using AuthenticationBusiness.Interfaces.Domains;
using AuthenticationDb.Interfaces;
using AuthenticationDb.Models.Entities;
using AuthenticationModel.Enumerations;
using AuthenticationModel.Models;
using AuthenticationShared.Resources;
using AuthenticationShared.ViewModels.Jwt;
using AuthenticationShared.ViewModels.User;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServiceShared.Exceptions;
using ServiceShared.Interfaces.Services;

namespace AuthenticationBusiness.Domains
{
    public class UserDomain : IUserDomain
    {
        #region Properties

        private readonly IEncryptionService _encryptionService;

        private readonly HttpContext _httpContext;

        private readonly IExternalAuthenticationService _externalAuthenticationService;

        private readonly IUnitOfWork _unitOfWork;

        private readonly ITimeService _timeService;

        //private readonly ApplicationSetting _applicationSettings;

        private readonly IRelationalDbService _relationalDbService;

        private readonly AppJwtModel _appJwt;

        #endregion

        #region Constructors

        public UserDomain(IEncryptionService encryptionService,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IExternalAuthenticationService externalAuthenticationService,
            ITimeService timeService,
            IRelationalDbService relationalDbService,
            IOptions<AppJwtModel> appJwt
            //ApplicationSetting applicationSettings
            )
        {
            _encryptionService = encryptionService;
            _unitOfWork = unitOfWork;
            _httpContext = httpContextAccessor.HttpContext;
            _externalAuthenticationService = externalAuthenticationService;
            _timeService = timeService;
            //_applicationSettings = applicationSettings;
            _relationalDbService = relationalDbService;
            _appJwt = appJwt.Value;
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
            jwt.Expiration = _timeService.DateTimeUtcToUnix(jwtExpiration);

            //_profileCacheService.Add(user.Id, user, LifeTimeConstant.JwtLifeTime);
            return jwt;
        }

        #endregion
    }
}
