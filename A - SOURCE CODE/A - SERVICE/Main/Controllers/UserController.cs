using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using SystemConstant.Enumerations;
using SystemConstant.Enumerations.Order;
using SystemConstant.Models;
using SystemDatabase.Interfaces;
using SystemDatabase.Interfaces.Repositories;
using SystemDatabase.Models.Entities;
using AutoMapper;
using Main.Constants;
using Main.Interfaces.Services;
using Main.Models;
using Main.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Interfaces.Services;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels;
using Shared.ViewModels.Accounts;
using SkiaSharp;
using VgySdk.Interfaces;
using VgySdk.Models;

namespace Main.Controllers
{
    [Route("api/[controller]")]
    public class UserController : ApiBaseController
    {
        #region Constructors

        /// <summary>
        ///     Initiate controller with injectors.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="mapper"></param>
        /// <param name="timeService"></param>
        /// <param name="dbSharedService"></param>
        /// <param name="encryptionService"></param>
        /// <param name="identityService"></param>
        /// <param name="systemTimeService"></param>
        /// <param name="externalAuthenticationService"></param>
        /// <param name="sendMailService"></param>
        /// <param name="emailCacheService"></param>
        /// <param name="jwtConfigurationOptions"></param>
        /// <param name="applicationSettings"></param>
        /// <param name="logger"></param>
        /// <param name="vgyService"></param>
        public UserController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ITimeService timeService,
            IDbSharedService dbSharedService,
            IEncryptionService encryptionService,
            IIdentityService identityService,
            ITimeService systemTimeService,
            IExternalAuthenticationService externalAuthenticationService,
            ISendMailService sendMailService,
            IEmailCacheService emailCacheService,
            IOptions<JwtConfiguration> jwtConfigurationOptions,
            IOptions<ApplicationSetting> applicationSettings,
            ILogger<UserController> logger,
            IVgyService vgyService) : base(unitOfWork, mapper, timeService, dbSharedService, identityService)
        {
            _encryptionService = encryptionService;
            _jwtConfiguration = jwtConfigurationOptions.Value;
            _applicationSettings = applicationSettings.Value;
            _logger = logger;
            _externalAuthenticationService = externalAuthenticationService;
            _unitOfWork = unitOfWork;
            _databaseFunction = dbSharedService;
            _identityService = identityService;
            _sendMailService = sendMailService;
            _emailCacheService = emailCacheService;
            _vgyService = vgyService;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Use specific condition to check whether account is available or not.
        ///     If account is valid for logging into system, access token will be provided.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost("basic-login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginViewModel parameters)
        {
            #region Parameters validation

            // Parameter hasn't been initialized.
            if (parameters == null)
            {
                parameters = new LoginViewModel();
                TryValidateModel(parameters);
            }

            // Invalid modelstate.
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

#if !ALLOW_ANONYMOUS

            #region Search account

            // Hash the password first.
            var hashedPassword = _encryptionService.Md5Hash(parameters.Password);

            // Search for account which is active and information is correct.
            var accounts = UnitOfWork.Accounts.Search();
            accounts = accounts.Where(x =>
                x.Email.Equals(parameters.Email, StringComparison.InvariantCultureIgnoreCase) &&
                x.Password.Equals(hashedPassword, StringComparison.InvariantCultureIgnoreCase) && x.Type == AccountType.Basic);

            // Find the first account in database.
            var account = await accounts.FirstOrDefaultAsync();
            if (account == null)
                return NotFound(new ApiResponse(HttpMessages.AccountIsNotFound));

            #endregion

            #region Account state validation

            switch (account.Status)
            {
                case AccountStatus.Pending:
                    return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse(HttpMessages.AccountIsPending));
                case AccountStatus.Disabled:
                    return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse(HttpMessages.AccountIsDisabled));
            }

            #endregion

#else
            var account = new Account();
            account.Email = "redplane_dt@yahoo.com.vn";
            account.Nickname = "Linh Nguyen";
#endif

            // Initialize jwt token.
            var jwt = InitializeAccountToken(account);
            return Ok(jwt);
        }

        /// <summary>
        /// Use specific conditions to login into Google authentication system.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost("google-login")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginViewModel info)
        {
            #region Parameters validation

            if (info == null)
            {
                info = new GoogleLoginViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Authentication request

            // Find token information.
            var tokenInfo = await _externalAuthenticationService.GetGoogleTokenInfoAsync(info.Code);
            if (tokenInfo == null || string.IsNullOrWhiteSpace(tokenInfo.Id))
                return StatusCode((int)HttpStatusCode.Forbidden, new ApiResponse(HttpMessages.GoogleCodeIsInvalid));

            // Get the profile information.
            var profile = await _externalAuthenticationService.GetGoogleBasicProfileAsync(tokenInfo.Id);
            if (profile == null)
                return StatusCode((int)HttpStatusCode.Forbidden, HttpMessages.GoogleCodeIsInvalid);

            #endregion

            #region Account availability check

            // Find accounts by searching for email address.
            var accounts = UnitOfWork.Accounts.Search();
            accounts = accounts.Where(x => x.Email.Equals(profile.Email));

            // Get the first matched account.
            var account = await accounts.FirstOrDefaultAsync();

            // Account is available in the system. Check its status.
            if (account != null)
            {
                // Prevent account from logging into system because it is pending.
                if (account.Status == AccountStatus.Pending)
                    return StatusCode((int)HttpStatusCode.Forbidden, new ApiResponse(HttpMessages.AccountIsPending));

                // Prevent account from logging into system because it is deleted.
                if (account.Status == AccountStatus.Disabled)
                    return StatusCode((int)HttpStatusCode.Forbidden, new ApiResponse(HttpMessages.AccountIsPending));
            }
            else
            {
                // Initialize account instance.
                account = new Account();
                account.Email = profile.Email;
                account.Nickname = profile.Name;
                account.Role = AccountRole.User;
                account.PhotoRelativeUrl = profile.Picture;
                account.JoinedTime = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);
                account.Type = AccountType.Google;

                // Add account to database.
                UnitOfWork.Accounts.Insert(account);
                await UnitOfWork.CommitAsync();
            }

            #endregion

            // Initialize access token.
            var jwt = InitializeAccountToken(account);
            return Ok(jwt);
        }

        /// <summary>
        /// Use specific information to sign into system using facebook account.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost("facebook-login")]
        [AllowAnonymous]
        public async Task<IActionResult> FacebookLogin([FromBody] FacebookLoginViewModel info)
        {
            #region Parameters validation

            // Information hasn't been initialized.
            if (info == null)
            {
                info = new FacebookLoginViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Authentication request

            // Find token information.
            var tokenInfo = await _externalAuthenticationService.GetFacebookTokenInfoAsync(info.Code);
            if (tokenInfo == null || string.IsNullOrWhiteSpace(tokenInfo.AccessToken))
                return StatusCode((int)HttpStatusCode.Forbidden, new ApiResponse(HttpMessages.FacebookCodeIsInvalid));

            // Get the profile information.
            var profile = await _externalAuthenticationService.GetFacebookBasicProfileAsync(tokenInfo.AccessToken);
            if (profile == null)
                return StatusCode((int)HttpStatusCode.Forbidden, HttpMessages.GoogleCodeIsInvalid);

            #endregion

            #region Account availability check

            // Find accounts by searching for email address.
            var accounts = UnitOfWork.Accounts.Search();
            accounts = accounts.Where(x => x.Email.Equals(profile.Email));

            // Get the first matched account.
            var account = await accounts.FirstOrDefaultAsync();

            // Account is available in the system. Check its status.
            if (account != null)
            {
                // Prevent account from logging into system because it is pending.
                if (account.Status == AccountStatus.Pending)
                    return StatusCode((int)HttpStatusCode.Forbidden, new ApiResponse(HttpMessages.AccountIsPending));

                // Prevent account from logging into system because it is deleted.
                if (account.Status == AccountStatus.Disabled)
                    return StatusCode((int)HttpStatusCode.Forbidden, new ApiResponse(HttpMessages.AccountIsPending));
            }
            else
            {
                // Initialize account instance.
                account = new Account();
                account.Email = profile.Email;
                account.Nickname = profile.FullName;
                account.Role = AccountRole.User;
                account.JoinedTime = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);
                account.Type = AccountType.Facebook;

                // Add account to database.
                UnitOfWork.Accounts.Insert(account);
                await UnitOfWork.CommitAsync();
            }

            #endregion

            // Initialize access token.
            var jwt = InitializeAccountToken(account);
            return Ok(jwt);

        }

        /// <summary>
        ///     Find personal profile.
        /// </summary>
        /// <param name="id">Id of user. 0 for the request sender profile.</param>
        /// <returns></returns>
        [HttpGet("personal-profile/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> FindProfile([FromRoute] int? id)
        {
            // Get requester identity.
            var profile = IdentityService.GetProfile(HttpContext);

            // Search for accounts.
            var accounts = UnitOfWork.Accounts.Search();

            if (id == null || id < 1)
            {
                if (profile != null)
                    accounts = accounts.Where(x => x.Id == profile.Id);
                else
                    return Ok();
            }
            else
                accounts = accounts.Where(x => x.Id == id);

            // Only search for active account.
            accounts = accounts.Where(x => x.Status == AccountStatus.Available);

            // Find the first account in system.
            var account = await accounts.FirstOrDefaultAsync();
            return Ok(account);
        }

        /// <summary>
        ///     Base on specific information to create an account in database.
        /// </summary>
        /// <returns></returns>
        [Route("basic-register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterAccountViewModel parameters)
        {
            #region Parameters validation

            // Parameters haven't been initialized. Initialize 'em.
            if (parameters == null)
            {
                parameters = new RegisterAccountViewModel();
                TryValidateModel(parameters);
            }

            // Parameters are invalid. Send errors back to client.
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Search for duplicate accounts.

            // Search for duplicated accounts.
            var accounts = UnitOfWork.Accounts.Search();
            accounts = accounts.Where(
                x => x.Email.Equals(parameters.Email, StringComparison.InvariantCultureIgnoreCase));

            // Find the first matched account.
            var account = await accounts.FirstOrDefaultAsync();

            // Account exists in system.
            if (account != null)
            {
                Response.StatusCode = (int)HttpStatusCode.Conflict;
                return Json(new ApiResponse(HttpMessages.AccountIsInUse));
            }

            #endregion

            #region Initiate account

            // Initiate account with specific information.
            account = new Account();
            account.Email = parameters.Email;
            account.Password = _encryptionService.Md5Hash(parameters.Password);
            account.Nickname = parameters.Nickname;

            // Add account into database.
            UnitOfWork.Accounts.Insert(account);

            // Save changes asychronously.
            await UnitOfWork.CommitAsync();

            #endregion

            #region Send email

            var emailTemplate = _emailCacheService.Read(EmailTemplateConstant.RegisterBasicAccount);

            if (emailTemplate != null)
            {
                await _sendMailService.SendAsync(new HashSet<string> { account.Email }, null, null, emailTemplate.Subject, emailTemplate.Content, false, CancellationToken.None);
            }


            // TODO: Implement notification service which notifies administrators about the registration.

            #endregion

            return Ok();
        }

        /// <summary>
        ///     Request service to send an instruction email to help user to reset his/her password.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel parameter)
        {
            #region Model validation

            // Parameter hasn't been initialized.
            if (parameter == null)
            {
                parameter = new ForgotPasswordViewModel();
                TryValidateModel(parameter);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Email search

            // Initiate search conditions.
            var conditions = new RequestPasswordViewModel();
            conditions.Email = new TextSearch(TextSearchMode.EndsWithIgnoreCase, parameter.Email);
            conditions.Statuses = new[] { AccountStatus.Available };

            // Search user in database.
            var accounts = UnitOfWork.Accounts.Search();
            accounts = accounts.Where(x =>
                x.Email.Equals(parameter.Email, StringComparison.InvariantCultureIgnoreCase) &&
                x.Status == AccountStatus.Available);

            // Find the first matched account.
            var account = await accounts.FirstOrDefaultAsync();

            // User is not found.
            if (account == null)
                return NotFound(HttpMessages.AccountIsNotFound);

            #endregion

            #region Information initialization

            // Find current system time.
            var systemTime = DateTime.UtcNow;
            var expiration = systemTime.AddSeconds(_applicationSettings.PasswordResetTokenLifeTime);

            // Initiate token.
            var token = new Token();
            token.OwnerId = account.Id;
            token.Type = TokenType.AccountReactiveCode;
            token.Code = Guid.NewGuid().ToString("D");
            token.IssuedTime = TimeService.DateTimeUtcToUnix(systemTime);
            token.ExpiredTime = TimeService.DateTimeUtcToUnix(expiration);

            // Save token into database.
            UnitOfWork.Tokens.Insert(token);

            #endregion

            #region Email broadcast

            var emailTemplate = _emailCacheService.Read(EmailTemplateConstant.ForgotPasswordRequest);

            if (emailTemplate != null)
            {
                await _sendMailService.SendAsync(new HashSet<string> { account.Email }, null, null, emailTemplate.Subject,
                    emailTemplate.Content, false, CancellationToken.None);

                _logger.LogInformation($"Sent message to {account.Email} with subject {emailTemplate.Subject}");
            }

            #endregion

            return Ok();
        }

        /// <summary>
        ///     Submit password by using forgot password token.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public async Task<IActionResult> SubmitPassword(
            [FromQuery] [Required(ErrorMessageResourceType = typeof(HttpValidationMessages),
                ErrorMessageResourceName = "InformationIsRequired")] string code,
            [FromBody] SubmitPasswordResetViewModel parameter)
        {
            throw new NotImplementedException();
            //#region Model validation

            //if (parameter == null)
            //{
            //    parameter = new SubmitPasswordResetViewModel();
            //    TryValidateModel(parameter);
            //}

            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);

            //#endregion

            //#region Information search

            //// Find active accounts.
            //var accounts = _unitOfWork.Accounts.Search();
            //accounts = accounts.Where(x => x.Status == AccountStatus.Available);

            //// Find active token.
            //var epochSystemTime = _systemTimeService.DateTimeUtcToUnix(DateTime.UtcNow);
            //var tokens = _unitOfWork.Tokens.Search();

            //// Find token.
            //var result = from account in accounts
            //    from token in tokens
            //    where account.Id == token.OwnerId && token.ExpiredTime < epochSystemTime
            //    select new SearchAccountTokenResult
            //    {
            //        Token = token,
            //        Account = account
            //    };

            //// No active token is found.
            //if (!await result.AnyAsync())
            //    return NotFound(HttpMessages.InformationNotFound);

            //#endregion

            //#region Information change

            //// Hash the password.
            //var password = _encryptionService.Md5Hash(parameter.Password);

            //// Delete all found tokens.
            //_unitOfWork.Tokens.Remove(result.Select(x => x.Token));
            //await result.ForEachAsync(x => x.Account.Password = password);

            //// Commit changes.
            //await _unitOfWork.CommitAsync();

            //#endregion

            //return Ok();
        }

        /// <summary>
        /// Initialize account access token.
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        private JwtResponse InitializeAccountToken(Account account)
        {
            // Find current time on the system.
            var systemTime = DateTime.Now;
            var jwtExpiration = systemTime.AddSeconds(_jwtConfiguration.LifeTime);

            // Claims initalization.
            var claims = InitUserClaim(account);

            // Write a security token.
            var jwtSecurityToken = new JwtSecurityToken(_jwtConfiguration.Issuer, _jwtConfiguration.Audience, claims,
                null, jwtExpiration, _jwtConfiguration.SigningCredentials);

            // Initiate token handler which is for generating token code.
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);

            // Initialize jwt response.
            var jwt = new JwtResponse();
            jwt.Code = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);
            jwt.LifeTime = _jwtConfiguration.LifeTime;
            jwt.Expiration = TimeService.DateTimeUtcToUnix(jwtExpiration);

            return jwt;
        }

        /// <summary>
        /// Initialize user claim.
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        private IList<Claim> InitUserClaim(Account account)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(JwtRegisteredClaimNames.Aud, _jwtConfiguration.Audience));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iss, _jwtConfiguration.Issuer));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, account.Email));
            claims.Add(new Claim(nameof(account.Nickname), account.Nickname));

            return claims;
        }

        /// <summary>
        /// Search for a list of accounts.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("search")]
        public async Task<IActionResult> SearchAccounts([FromBody] SearchAccountViewModel condition)
        {
            #region Parameters validation

            if (condition == null)
            {
                condition = new SearchAccountViewModel();
                TryValidateModel(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Search for information

            // Get all accounts
            var accounts = _unitOfWork.Accounts.Search();
            accounts = SearchAccounts(accounts, condition);

            // Sort by properties.
            if (condition.Sort != null)
                accounts =
                    _databaseFunction.Sort(accounts, condition.Sort.Direction,
                        condition.Sort.Property);
            else
                accounts = _databaseFunction.Sort(accounts, SortDirection.Decending,
                    AccountSort.JoinedTime);

            // Result initialization.
            var result = new SearchResult<IList<Account>>();
            result.Total = await accounts.CountAsync();
            result.Records = await _databaseFunction.Paginate(accounts, condition.Pagination).ToListAsync();

            #endregion

            return Ok(result);
        }

        /// <summary>
        ///     Search accounts by using specific conditions.
        /// </summary>
        /// <param name="accounts"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public IQueryable<Account> SearchAccounts(IQueryable<Account> accounts,
            SearchAccountViewModel conditions)
        {
            if (conditions == null)
                return accounts;

            // Find identity in request.
            var identity = _identityService.GetProfile(HttpContext);

            // Id has been defined.
            if (conditions.Id != null)
                accounts = accounts.Where(x => x.Id == conditions.Id.Value);

            // Email search condition has been defined.
            if (conditions.Email != null && !string.IsNullOrWhiteSpace(conditions.Email))
                accounts = _databaseFunction.SearchPropertyText(accounts, x => x.Email,
                    new TextSearch(TextSearchMode.ContainIgnoreCase, conditions.Email));

            // Search conditions which are based on roles.
            if (identity?.Role == AccountRole.Admin)
            {
                // Statuses are defined.
                if (conditions.Statuses != null && conditions.Statuses.Count > 0)
                    accounts = accounts.Where(x => conditions.Statuses.Contains(x.Status));

                // Roles are defined
                if (conditions.Roles != null && conditions.Roles.Count > 0)
                    accounts = accounts.Where(x => conditions.Roles.Contains(x.Role));
            }

            return accounts;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("load-users")]
        public async Task<IActionResult> LoadUsers([FromBody] SearchUserViewModel condition)
        {
            #region Parameters validation

            if (condition == null)
            {
                condition = new SearchUserViewModel();
                TryValidateModel(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Search for information

            // Get all users
            var accounts = _unitOfWork.Accounts.Search();
            accounts = LoadUsers(accounts, condition);

            // Sort by properties.
            if (condition.Sort != null)
                accounts =
                    _databaseFunction.Sort(accounts, condition.Sort.Direction,
                        condition.Sort.Property);
            else
                accounts = _databaseFunction.Sort(accounts, SortDirection.Decending,
                    AccountSort.JoinedTime);

            // Result initialization.
            var result = new SearchResult<IList<Account>>();
            result.Total = await accounts.CountAsync();
            result.Records = await _databaseFunction.Paginate(accounts, condition.Pagination).ToListAsync();

            #endregion

            return Ok(result);
        }

        /// <summary>
        ///     Load accounts by using specific conditions.
        /// </summary>
        /// <param name="accounts"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public IQueryable<Account> LoadUsers(IQueryable<Account> accounts,
            SearchUserViewModel conditions)
        {
            if (conditions == null)
                return accounts;

            // Id has been defined.
            if (conditions.Ids != null && conditions.Ids.Count > 0)
            {
                conditions.Ids = conditions.Ids.Where(x => x > 0).ToList();
                if (conditions.Ids.Count > 0)
                    accounts = accounts.Where(x => conditions.Ids.Contains(x.Id));
            }

            return accounts;
        }

        /// <summary>
        /// Upload Avatar
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost("upload-avatar")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadAvatar(UploadPhotoViewModel info)
        {
            #region Parameters Validation

            if (info == null)
            {
                info = new UploadPhotoViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            // Get requester profile.
            var profile = IdentityService.GetProfile(HttpContext);

            #region Image proccessing

            // Reflect image variable.
            var image = info.Image;

            using (var skManagedStream = new SKManagedStream(image.OpenReadStream()))
            {
                var skBitmap = SKBitmap.Decode(skManagedStream);

                try
                {
                    // Resize image to 512x512 size.
                    var resizedSkBitmap = skBitmap.Resize(new SKImageInfo(512, 512), SKBitmapResizeMethod.Lanczos3);

                    // Initialize file name.
                    var fileName = $"{Guid.NewGuid():D}.png";

                    using (var skImage = SKImage.FromBitmap(resizedSkBitmap))
                    using (var skData = skImage.Encode(SKEncodedImageFormat.Png, 100))
                    using (var memoryStream = new MemoryStream())
                    {
                        skData.SaveTo(memoryStream);
                        var vgySuccessRespone = await _vgyService.UploadAsync<VgySuccessResponse>(memoryStream.ToArray(),
                            image.ContentType, fileName,
                            CancellationToken.None);

                        // Response is empty.
                        if (vgySuccessRespone == null || vgySuccessRespone.IsError)
                            return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse(HttpMessages.ImageIsInvalid));

                        profile.PhotoRelativeUrl = vgySuccessRespone.ImageUrl;
                        profile.PhotoAbsoluteUrl = vgySuccessRespone.ImageDeleteUrl;
                    }
                    
                    // Save changes into database.
                    await _unitOfWork.CommitAsync();

                    return Ok(profile);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception.Message, exception);
                    return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse(HttpMessages.ImageIsInvalid));
                }

                #endregion
            }
        }

        /// <summary>
        ///  Request service to send another email to obtain new account activation code.
        /// </summary>
        /// <returns></returns>
        [HttpPost("resend-activation-code")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendAccountActivationCode([FromBody] ResendActivationCodeViewModel info)
        {
            #region Parameters validation

            if (info == null)
            {
                info = new ResendActivationCodeViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Search for account

            var accounts = _unitOfWork.Accounts.Search();
            accounts = accounts.Where(x => x.Email.Equals(info.Email) && x.Status == AccountStatus.Pending && x.Type == AccountType.Basic);

            // Find the first matched account.
            var account = await accounts.FirstOrDefaultAsync();

            // User is not found.
            if (account == null)
                return NotFound(new ApiResponse(HttpMessages.AccountIsNotFound));

            #endregion

            #region Token generation

            // Find the existing token.
            // TODO: Generate new code.

            #endregion



            #region Send email

            var emailTemplate = _emailCacheService.Read(EmailTemplateConstant.ResendAccountActivationCode);

            if (emailTemplate != null)
                await _sendMailService.SendAsync(new HashSet<string> { account.Email }, null, null, emailTemplate.Subject, emailTemplate.Content, false, CancellationToken.None);
            
            #endregion

            return Ok();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Provides functions to encrypt/decrypt data.
        /// </summary>
        private readonly IEncryptionService _encryptionService;

        /// <summary>
        ///     Configuration information of JWT.
        /// </summary>
        private readonly JwtConfiguration _jwtConfiguration;

        /// <summary>
        ///     Collection of settings in application.
        /// </summary>
        private readonly ApplicationSetting _applicationSettings;

        /// <summary>
        /// Logging instance.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Service which is for handling external authentication service.
        /// </summary>
        private readonly IExternalAuthenticationService _externalAuthenticationService;

        /// <summary>
        ///     Instance for accessing database.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Provide access to generic database functions.
        /// </summary>
        private readonly IDbSharedService _databaseFunction;

        /// <summary>
        /// Instance which is for accessing identity attached in request.
        /// </summary>
        private readonly IIdentityService _identityService;

        /// <summary>
        /// Send email service
        /// </summary>
        private readonly ISendMailService _sendMailService;

        /// <summary>
        /// Email cache service.
        /// </summary>
        private readonly IEmailCacheService _emailCacheService;

        /// <summary>
        /// Service which is for handling file upload to vgy.me hosting.
        /// </summary>
        private readonly IVgyService _vgyService;

        #endregion
    }
}