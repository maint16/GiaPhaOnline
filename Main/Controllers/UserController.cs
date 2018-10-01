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
using AppDb.Interfaces;
using AppDb.Interfaces.Repositories;
using AppDb.Models.Entities;
using AppModel.Enumerations;
using AppModel.Enumerations.Order;
using AppModel.Models;
using AutoMapper;
using Main.Authentications.ActionFilters;
using Main.Constants;
using Main.Interfaces.Services;
using Main.Models;
using Main.Models.Jwt;
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
        /// <param name="relationalDbService"></param>
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
        /// <param name="profileCacheService"></param>
        /// <param name="captchaService"></param>
        public UserController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ITimeService timeService,
            IRelationalDbService relationalDbService,
            IEncryptionService encryptionService,
            IIdentityService identityService,
            ITimeService systemTimeService,
            IExternalAuthenticationService externalAuthenticationService,
            ISendMailService sendMailService,
            IEmailCacheService emailCacheService,
            IOptions<JwtConfiguration> jwtConfigurationOptions,
            IOptions<ApplicationSetting> applicationSettings,
            ILogger<UserController> logger,
            IVgyService vgyService,
            IValueCacheService<int, User> profileCacheService,
            ICaptchaService captchaService) : base(unitOfWork, mapper, timeService,
            relationalDbService, identityService)
        {
            _encryptionService = encryptionService;
            _jwtConfiguration = jwtConfigurationOptions.Value;
            _applicationSettings = applicationSettings.Value;
            _logger = logger;
            _externalAuthenticationService = externalAuthenticationService;
            _unitOfWork = unitOfWork;
            _databaseFunction = relationalDbService;
            _identityService = identityService;
            _sendMailService = sendMailService;
            _emailCacheService = emailCacheService;
            _systemTimeService = systemTimeService;
            _vgyService = vgyService;
            _profileCacheService = profileCacheService;
            _captchaService = captchaService;
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

#if !DISABLE_CAPTCHA_VALIDATION
            // Verify the captcha.
            var bIsCaptchaValid = await _captchaService.IsCaptchaValidAsync(parameters.CaptchaCode, null, CancellationToken.None);
            if (!bIsCaptchaValid)
                return StatusCode((int)HttpStatusCode.Forbidden, new ApiResponse(HttpMessages.CaptchaInvalid));
#endif

#if !ALLOW_ANONYMOUS

            #region Search account

            // Hash the password first.
            var hashedPassword = _encryptionService.Md5Hash(parameters.Password);

            // Search for account which is active and information is correct.
            var accounts = UnitOfWork.Accounts.Search();
            accounts = accounts.Where(x =>
                x.Email.Equals(parameters.Email, StringComparison.InvariantCultureIgnoreCase) &&
                x.Password.Equals(hashedPassword, StringComparison.InvariantCultureIgnoreCase) && x.Type == UserKind.Basic);

            // Find the first account in database.
            var account = await accounts.FirstOrDefaultAsync();
            if (account == null)
                return NotFound(new ApiResponse(HttpMessages.AccountIsNotFound));

            #endregion

            #region Account state validation

            switch (account.Status)
            {
                case UserStatus.Pending:
                    return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse(HttpMessages.AccountIsPending));
                case UserStatus.Disabled:
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
            
            // Get the profile information.
            var profile = await _externalAuthenticationService.GetGoogleBasicProfileAsync(info.IdToken);
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
                if (account.Status == UserStatus.Pending)
                    return StatusCode((int)HttpStatusCode.Forbidden, new ApiResponse(HttpMessages.AccountIsPending));

                // Prevent account from logging into system because it is deleted.
                if (account.Status == UserStatus.Disabled)
                    return StatusCode((int)HttpStatusCode.Forbidden, new ApiResponse(HttpMessages.AccountIsPending));
            }
            else
            {
                // Initialize account instance.
                account = new User();

#if USE_IN_MEMORY
                account.Id = UnitOfWork.Accounts.Search().OrderByDescending(x => x.Id).Select(x => x.Id)
                                 .FirstOrDefault() + 1;
#endif
                account.Email = profile.Email;
                account.Nickname = profile.Name;
                account.Role = UserRole.User;
                account.Photo = profile.Picture;
                account.JoinedTime = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);
                account.Type = UserKind.Google;
                account.Status = UserStatus.Available;

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
            var tokenInfo = await _externalAuthenticationService.GetFacebookTokenInfoAsync(info.AccessToken);
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
                if (account.Status == UserStatus.Pending)
                    return StatusCode((int)HttpStatusCode.Forbidden, new ApiResponse(HttpMessages.AccountIsPending));

                // Prevent account from logging into system because it is deleted.
                if (account.Status == UserStatus.Disabled)
                    return StatusCode((int)HttpStatusCode.Forbidden, new ApiResponse(HttpMessages.AccountIsPending));
            }
            else
            {
                // Initialize account instance.
                account = new User();
                account.Email = profile.Email;
                account.Nickname = profile.FullName;
                account.Role = UserRole.User;
                account.JoinedTime = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);
                account.Type = UserKind.Facebook;

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
        [ByPassAuthorization]
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
                    return NotFound();
            }
            else
                accounts = accounts.Where(x => x.Id == id);

            // Only search for active account.
            // Admin can see deactivated account.
            if (profile != null && profile.Role != UserRole.Admin)
                accounts = accounts.Where(x => x.Status == UserStatus.Available);

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

#if !DISABLE_CAPTCHA_VALIDATION
            // Verify the captcha.
            var bIsCaptchaValid = await _captchaService.IsCaptchaValidAsync(parameters.CaptchaCode, null, CancellationToken.None);
            if (!bIsCaptchaValid)
                return StatusCode((int)HttpStatusCode.Forbidden, new ApiResponse(HttpMessages.CaptchaInvalid));
#endif

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

            #region Add user & user activation code

            using (var transactionScope = UnitOfWork.BeginTransactionScope())
            {
                // Initiate account with specific information.
                account = new User();
                account.Email = parameters.Email;
                account.Password = _encryptionService.Md5Hash(parameters.Password);
                account.Nickname = parameters.Nickname;

                // Add account into database.
                UnitOfWork.Accounts.Insert(account);

                var activationToken = new ActivationToken();
                activationToken.OwnerId = account.Id;
                activationToken.Code = Guid.NewGuid().ToString("D");
                activationToken.IssuedTime = _systemTimeService.DateTimeUtcToUnix(DateTime.UtcNow);
                activationToken.ExpiredTime = activationToken.IssuedTime + 3600;
                UnitOfWork.ActivationTokens.Insert(activationToken);

                // Commit the transaction.
                _unitOfWork.Commit();
                transactionScope.Commit();
            }
             
            #endregion


            #region Background tasks execution

            // Initialize background tasks.
            var backgroundTasks = new List<Task>();

            var emailTemplate = _emailCacheService.Read(EmailTemplateConstant.RegisterBasicAccount);
            if (emailTemplate != null)
            {
                var pSendMailTask = _sendMailService.SendAsync(new HashSet<string> {account.Email}, null, null,
                    emailTemplate.Subject, emailTemplate.Content, emailTemplate.IsHtmlContent, CancellationToken.None);
                backgroundTasks.Add(pSendMailTask);
            }

            // TODO: Implement notification service which notifies administrators about the registration.
            
            //var pSendRealTimeNotificationTask = _pusherService.SendAsync()
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

            // Verify the captcha.
            var bIsCaptchaValid = await _captchaService.IsCaptchaValidAsync(parameter.CaptchaCode, null, CancellationToken.None);
            if (!bIsCaptchaValid)
                return StatusCode((int)HttpStatusCode.Forbidden, new ApiResponse(HttpMessages.CaptchaInvalid));

            #region Email search

            // Initiate search conditions.
            //            var conditions = new RequestPasswordViewModel();
            //            conditions.Email = new TextSearch(TextSearchMode.EndsWithIgnoreCase, parameter.Email);
            //            conditions.Statuses = new[] { UserStatus.Available };

            // Search user in database.
            var accounts = UnitOfWork.Accounts.Search();
            accounts = accounts.Where(x =>
                x.Email.Equals(parameter.Email, StringComparison.InvariantCultureIgnoreCase) &&
                x.Status == UserStatus.Available);

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
            var token = new AccessToken();
            token.OwnerId = account.Id;
            //token.Type = TokenType.AccountReactiveCode;
            token.Code = Guid.NewGuid().ToString("D");
            token.IssuedTime = TimeService.DateTimeUtcToUnix(systemTime);
            token.ExpiredTime = TimeService.DateTimeUtcToUnix(expiration);

            // Save token into database.
            UnitOfWork.AccessTokens.Insert(token);

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
        [HttpPost("submit-password-reset")]
        [AllowAnonymous]
        public async Task<IActionResult> SubmitPassword(
            [FromQuery] [Required(ErrorMessageResourceType = typeof(HttpValidationMessages),
                ErrorMessageResourceName = "InformationIsRequired")] string code,
            [FromBody] SubmitPasswordResetViewModel parameter)
        {
            #region Model validation

            if (parameter == null)
            {
                parameter = new SubmitPasswordResetViewModel();
                TryValidateModel(parameter);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            //#region Information search

            //// Find active accounts.
            //var accounts = _unitOfWork.Accounts.Search();
            //accounts = accounts.Where(x => x.Email.Equals(parameter.Email, StringComparison.InvariantCultureIgnoreCase) && x.Status == UserStatus.Available);

            //// Find active token.
            //var epochSystemTime = _systemTimeService.DateTimeUtcToUnix(DateTime.UtcNow);
            //var tokens = _unitOfWork.AccessTokens.Search();

            //// Find token.
            //var result = from account in accounts
            //             from token in tokens
            //             where account.Id == token.OwnerId && token.ExpiredTime < epochSystemTime
            //             select new SearchAccountTokenResult
            //             {
            //                 Token = token,
            //                 Account = account
            //             };

            //// No active token is found.
            //if (!await result.AnyAsync())
            //    return NotFound(HttpMessages.InformationNotFound);

            //#endregion

            //#region Information change

            //// Hash the password.
            //var password = _encryptionService.Md5Hash(parameter.Password);

            //// Delete all found tokens.
            //_unitOfWork.AccessTokens.Remove(result.Select(x => x.Token));
            //await result.ForEachAsync(x => x.Account.Password = password);

            //// Commit changes.
            //await _unitOfWork.CommitAsync();

            //#endregion

            //#region Send email

            //// Find the first matched account.
            //var accountSendMail = await accounts.FirstOrDefaultAsync();

            //var emailTemplate = _emailCacheService.Read(EmailTemplateConstant.SubmitPasswordReset);

            //if (emailTemplate != null)
            //    await _sendMailService.SendAsync(new HashSet<string> { accountSendMail.Email }, null, null, emailTemplate.Subject, emailTemplate.Content, false, CancellationToken.None);

            //#endregion
            throw new NotImplementedException();
            return Ok();
        }

        /// <summary>
        /// Using information submitted by user to change account password.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost("change-password/{id}")]
        public async Task<IActionResult> ChangePassword([FromRoute] int id, [FromBody] ChangePasswordViewModel info)
        {
            #region Parmeters validation

            if (info == null)
            {
                info = new ChangePasswordViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Find account

            // Hash the curent password.
            var hashedCurrentPassword = _encryptionService.Md5Hash(info.OriginalPassword);

            // Get user profile.
            var profile = _identityService.GetProfile(HttpContext);

            // Invalid index. That means current user wants to change his/her account password.
            if (id < 1 || profile.Id == id)
            {
                // Check current password.
                if (profile.Password != hashedCurrentPassword)
                    return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse(HttpMessages.CurrentPasswordIsInvalid));

                profile.Password = _encryptionService.Md5Hash(info.Password);
            }
            else
            {
                var accounts = _unitOfWork.Accounts.Search();
                accounts = accounts.Where(x => x.Id == id && x.Status == UserStatus.Available);

                // Get the first matched account
                var account = await accounts.FirstOrDefaultAsync();
                if (account == null)
                    return NotFound(new ApiResponse(HttpMessages.AccountIsNotFound));

                account.Password = _encryptionService.Md5Hash(info.Password);
            }

            await UnitOfWork.CommitAsync();

            #endregion

            return Ok();
        }

        /// <summary>
        /// Change user status by searching for user id.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("status/{id}")]
        [Authorize(Policy = PolicyConstant.IsAdminPolicy)]
        public async Task<IActionResult> ChangeUserStatus([FromRoute] int id, [FromBody] ChangeUserStatusViewModel info)
        {
            // Find requester profile.
            var profile = IdentityService.GetProfile(HttpContext);

            // User id is the same as the requester id. This is not allowed because user cannot change his/her account status.
            if (profile.Id == id)
                return StatusCode((int)HttpStatusCode.Forbidden,
                    new ApiResponse(HttpMessages.CannotChangeOwnProfileStatus));

            // Find user by using index.
            var users = _unitOfWork.Accounts.Search();
            users = users.Where(x => x.Id == id);

            // Find the first record in database.
            var user = await users.FirstOrDefaultAsync();
            if (user == null)
                return NotFound(new ApiResponse(HttpMessages.AccountIsNotFound));

            #region Information update

            // Whether information has been changed or not.
            var bHasInformationChanged = false;

            // Status has been defined.
            if (info.Status != user.Status)
            {
                if (info.Status == UserStatus.Pending)
                {
                    user.Status = UserStatus.Disabled;
                    bHasInformationChanged = true;
                }
                else
                {
                    user.Status = info.Status;
                    bHasInformationChanged = true;
                }

            }

            //todo: Reason

            // Information has been changed.
            if (bHasInformationChanged)
                await _unitOfWork.CommitAsync();

            #endregion
            return Ok();
        }
        
        /// <summary>
        ///     Load accounts by using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("search")]
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

            // Find identity in request.
            var identity = _identityService.GetProfile(HttpContext);

            #region Search for information

            // Get all users
            var accounts = _unitOfWork.Accounts.Search();

            // Id have been defined.
            if (condition.Ids != null && condition.Ids.Count > 0)
            {
                condition.Ids = condition.Ids.Where(x => x > 0).ToList();
                if (condition.Ids != null && condition.Ids.Count > 0)
                {
                    accounts = accounts.Where(x => condition.Ids.Contains(x.Id));
                }
            }

            // Email have been defined.
            if (condition.Emails != null && condition.Emails.Count > 0)
            {
                condition.Emails = condition.Emails.Where(x => !string.IsNullOrEmpty(x)).ToList();
                if (condition.Emails != null && condition.Emails.Count > 0)
                {
                    accounts = accounts.Where(x => condition.Emails.Any(y => x.Email.Contains(y)));
                }
            }

            // Search conditions which are based on roles.

            if (identity?.Role == UserRole.Admin)
            {
                // Statuses have been defined.
                if (condition.Statuses != null && condition.Statuses.Count > 0)
                {
                    condition.Statuses =
                        condition.Statuses.Where(x => Enum.IsDefined(typeof(UserStatus), x)).ToList();
                    if (condition.Statuses.Count > 0)
                        accounts = accounts.Where(x => condition.Statuses.Contains(x.Status));
                }

                // Roles have been defined.
                if (condition.Roles != null && condition.Roles.Count > 0)
                {
                    condition.Roles =
                        condition.Roles.Where(x => Enum.IsDefined(typeof(UserRole), x)).ToList();
                    if (condition.Roles.Count > 0)
                        accounts = accounts.Where(x => condition.Roles.Contains(x.Role));
                }
            }

            #endregion

            // Sort by properties.
            if (condition.Sort != null)
                accounts =
                    _databaseFunction.Sort(accounts, condition.Sort.Direction,
                        condition.Sort.Property);
            else
                accounts = _databaseFunction.Sort(accounts, SortDirection.Decending,
                    AccountSort.JoinedTime);

            // Result initialization.
            var result = new SearchResult<IList<User>>();
            result.Total = await accounts.CountAsync();
            result.Records = await _databaseFunction.Paginate(accounts, condition.Pagination).ToListAsync();

            return Ok(result);
        }

        //        /// <summary>
        //        /// Upload Avatar
        //        /// </summary>
        //        /// <param name="info"></param>
        //        /// <returns></returns>
        //        [HttpPost("upload-avatar")]
        //        [Consumes("multipart/form-data")]
        //        public async Task<IActionResult> UploadAvatar(UploadPhotoViewModel info)
        //        {
        //            #region Parameters Validation
        //
        //            if (info == null)
        //            {
        //                info = new UploadPhotoViewModel();
        //                TryValidateModel(info);
        //            }
        //
        //            if (!ModelState.IsValid)
        //                return BadRequest(ModelState);
        //
        //            #endregion
        //
        //            // Get requester profile.
        //            var profile = IdentityService.GetProfile(HttpContext);
        //
        //            #region Image proccessing
        //
        //            // Reflect image variable.
        //            var image = info.Image;
        //
        //            using (var skManagedStream = new SKManagedStream(image.OpenReadStream()))
        //            {
        //                var skBitmap = SKBitmap.Decode(skManagedStream);
        //
        //                try
        //                {
        //                    // Resize image to 512x512 size.
        //                    var resizedSkBitmap = skBitmap.Resize(new SKImageInfo(512, 512), SKBitmapResizeMethod.Lanczos3);
        //
        //                    // Initialize file name.
        //                    var fileName = $"{Guid.NewGuid():D}.png";
        //
        //                    using (var skImage = SKImage.FromBitmap(resizedSkBitmap))
        //                    using (var skData = skImage.Encode(SKEncodedImageFormat.Png, 100))
        //                    using (var memoryStream = new MemoryStream())
        //                    {
        //                        skData.SaveTo(memoryStream);
        //                        var vgySuccessRespone = await _vgyService.UploadAsync<VgySuccessResponse>(memoryStream.ToArray(),
        //                            image.ContentType, fileName,
        //                            CancellationToken.None);
        //
        //                        // Response is empty.
        //                        if (vgySuccessRespone == null || vgySuccessRespone.IsError)
        //                            return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse(HttpMessages.ImageIsInvalid));
        //
        //                        profile.PhotoRelativeUrl = vgySuccessRespone.ImageUrl;
        //                        profile.PhotoAbsoluteUrl = vgySuccessRespone.ImageDeleteUrl;
        //                    }
        //
        //                    // Save changes into database.
        //                    await _unitOfWork.CommitAsync();
        //
        //                    return Ok(profile);
        //                }
        //                catch (Exception exception)
        //                {
        //                    _logger.LogError(exception.Message, exception);
        //                    return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse(HttpMessages.ImageIsInvalid));
        //                }
        //
        //                #endregion
        //            }
        //        }

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
            accounts = accounts.Where(x => x.Email.Equals(info.Email) && x.Status == UserStatus.Pending && x.Type == UserKind.Basic);

            // Find the first matched account.
            var account = await accounts.FirstOrDefaultAsync();

            // User is not found.
            if (account == null)
                return NotFound(new ApiResponse(HttpMessages.AccountIsNotFound));

            #endregion

            #region Token generation

            // Find the existing token.
            var tokens = _unitOfWork.AccessTokens.Search();
            tokens = tokens.Where(x => x.OwnerId == account.Id);

            // Find the first matched token.
            var token = await tokens.FirstOrDefaultAsync();

            if (token != null)
                _unitOfWork.AccessTokens.Remove(token);
            else
            {
                // Find current system time.
                var systemTime = DateTime.UtcNow;
                var expiration = systemTime.AddSeconds(_applicationSettings.PasswordResetTokenLifeTime);

                token = new AccessToken();
                token.Code = Guid.NewGuid().ToString("D");
                token.OwnerId = account.Id;
                token.IssuedTime = TimeService.DateTimeUtcToUnix(systemTime);
                token.ExpiredTime = TimeService.DateTimeUtcToUnix(expiration);

                // Add token into database
                _unitOfWork.AccessTokens.Insert(token);
            }

            // Save changes asychronously.
            await UnitOfWork.CommitAsync();

            #endregion

            #region Send email

            var emailTemplate = _emailCacheService.Read(EmailTemplateConstant.ResendAccountActivationCode);

            if (emailTemplate != null)
                await _sendMailService.SendAsync(new HashSet<string> { account.Email }, null, null, emailTemplate.Subject, emailTemplate.Content, false, CancellationToken.None);

            #endregion

            return Ok();
        }
        
        /// <summary>
        /// Initialize account access token.
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        private JwtResponse InitializeAccountToken(User account)
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

            _profileCacheService.Add(account.Id, account, LifeTimeConstant.JwtLifeTime);
            return jwt;
        }

        /// <summary>
        /// Initialize user claim.
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        private IList<Claim> InitUserClaim(User account)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(JwtRegisteredClaimNames.Aud, _jwtConfiguration.Audience));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iss, _jwtConfiguration.Issuer));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, account.Email));
            claims.Add(new Claim(nameof(account.Nickname), account.Nickname));
            claims.Add(new Claim(nameof(account.Id), account.Id.ToString()));

            return claims;
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
        ///     Logging instance.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        ///     Service which is for handling external authentication service.
        /// </summary>
        private readonly IExternalAuthenticationService _externalAuthenticationService;

        /// <summary>
        ///     Instance for accessing database.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        ///     Provide access to generic database functions.
        /// </summary>
        private readonly IRelationalDbService _databaseFunction;

        /// <summary>
        ///     Instance which is for accessing identity attached in request.
        /// </summary>
        private readonly IIdentityService _identityService;

        /// <summary>
        ///     Send email service
        /// </summary>
        private readonly ISendMailService _sendMailService;

        /// <summary>
        ///     Email cache service.
        /// </summary>
        private readonly IEmailCacheService _emailCacheService;

        /// <summary>
        ///     System time service
        /// </summary>
        private readonly ITimeService _systemTimeService;

        /// <summary>
        ///     Service which is for handling file upload to vgy.me hosting.
        /// </summary>
        private readonly IVgyService _vgyService;

        /// <summary>
        ///     Service which is for handling profile caching.
        /// </summary>
        private readonly IValueCacheService<int, User> _profileCacheService;

        /// <summary>
        ///     Service which is for checking captcha.
        /// </summary>
        private readonly ICaptchaService _captchaService;
        
        #endregion
    }
}