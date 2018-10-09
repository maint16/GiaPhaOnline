using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AppDb.Interfaces;
using AppDb.Models.Entities;
using AppModel.Enumerations;
using AutoMapper;
using Main.Authentications.ActionFilters;
using Main.Constants;
using Main.Constants.RealTime;
using Main.Interfaces.Services;
using Main.Interfaces.Services.Businesses;
using Main.Interfaces.Services.RealTime;
using Main.Models;
using Main.Models.Jwt;
using Main.Models.RealTime;
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
using VgySdk.Interfaces;

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
        /// <param name="realTimeService"></param>
        /// <param name="userService"></param>
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
            ICaptchaService captchaService, IRealTimeService realTimeService, IUserService userService) : base(
            unitOfWork, mapper, timeService,
            relationalDbService, identityService)
        {
            _encryptionService = encryptionService;
            _jwtConfiguration = jwtConfigurationOptions.Value;
            _applicationSettings = applicationSettings.Value;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _sendMailService = sendMailService;
            _emailCacheService = emailCacheService;
            _profileCacheService = profileCacheService;
            _captchaService = captchaService;
            _realTimeService = realTimeService;
            _userService = userService;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Use specific condition to check whether account is available or not.
        ///     If account is valid for logging into system, access token will be provided.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("basic-login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            #region Parameters validation

            // Parameter hasn't been initialized.
            if (model == null)
            {
                model = new LoginViewModel();
                TryValidateModel(model);
            }

            // Invalid modelstate.
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            // Verify the captcha.
            var bIsCaptchaValid =
                await _captchaService.IsCaptchaValidAsync(model.CaptchaCode, null, CancellationToken.None);
            if (!bIsCaptchaValid)
                return StatusCode((int) HttpStatusCode.Forbidden, new ApiResponse(HttpMessages.CaptchaInvalid));

            var user = await _userService.LoginAsync(model);

            // Initialize jwt token.
            var jwt = InitializeAccountToken(user);
            return Ok(jwt);
        }

        /// <summary>
        ///     Use specific conditions to login into Google authentication system.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost("google-login")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginViewModel info)
        {
            if (info == null)
            {
                info = new GoogleLoginViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.GoogleLoginAsync(info);

            // Initialize access token.
            var jwt = InitializeAccountToken(user);
            return Ok(jwt);
        }

        /// <summary>
        ///     Use specific information to sign into system using facebook account.
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

            // Do facebook login.
            var user = await _userService.FacebookLoginAsync(info);

            // Initialize access token.
            var jwt = InitializeAccountToken(user);
            return Ok(jwt);
        }

        /// <summary>
        ///     Find personal profile.
        /// </summary>
        /// <param name="id">Id of user. 0 for the request sender profile.</param>
        /// <returns></returns>
        [HttpGet("personal-profile/{id}")]
        [HttpGet("{id}")]
        [ByPassAuthorization]
        public async Task<IActionResult> FindProfile([FromRoute] int? id)
        {
            // Get requester identity.
            var profile = IdentityService.GetProfile(HttpContext);

            var loadUserCondition = new SearchUserViewModel();
            loadUserCondition.Ids = new HashSet<int>();

            if (id == null || id < 1)
                if (profile != null)
                    loadUserCondition.Ids.Add(profile.Id);
                else

                    return NotFound();
            else
                loadUserCondition.Ids.Add(id.Value);

            // Only search for active account.
            // Admin can see deactivated account.
            if (profile != null && profile.Role != UserRole.Admin)
            {
                loadUserCondition.Statuses = new HashSet<UserStatus>();
                loadUserCondition.Statuses.Add(UserStatus.Available);
            }

            // Find the first account in system.
            var user = await _userService.SearchUserAsync(loadUserCondition);
            return Ok(user);
        }

        /// <summary>
        ///     Base on specific information to create an account in database.
        /// </summary>
        /// <returns></returns>
        [Route("basic-register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterAccountViewModel model)
        {
            // Parameters haven't been initialized. Initialize 'em.
            if (model == null)
            {
                model = new RegisterAccountViewModel();
                TryValidateModel(model);
            }

            // Parameters are invalid. Send errors back to client.
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verify the captcha.
            var bIsCaptchaValid =
                await _captchaService.IsCaptchaValidAsync(model.CaptchaCode, null, CancellationToken.None);
            if (!bIsCaptchaValid)
                return StatusCode((int) HttpStatusCode.Forbidden, new ApiResponse(HttpMessages.CaptchaInvalid));

            var basicRegisterResult = await _userService.BasicRegisterAsync(model);

            // Initialize background tasks.
            var backgroundTasks = new List<Task>();

            var emailTemplate = _emailCacheService.Read(EmailTemplateConstant.RegisterBasicAccount);
            if (emailTemplate != null)
            {
                var pSendMailTask = _sendMailService.SendAsync(new HashSet<string> {basicRegisterResult.Email}, null,
                    null,
                    emailTemplate.Subject, emailTemplate.Content, emailTemplate.IsHtmlContent, CancellationToken.None);
                backgroundTasks.Add(pSendMailTask);
            }

            // TODO: Implement notification service which notifies administrators about the registration.


            return Ok();
        }

        /// <summary>
        ///     Request service to send an instruction email to help user to reset his/her password.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel model)
        {
            // Parameter hasn't been initialized.
            if (model == null)
            {
                model = new ForgotPasswordViewModel();
                TryValidateModel(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verify the captcha.
            var bIsCaptchaValid =
                await _captchaService.IsCaptchaValidAsync(model.CaptchaCode, null, CancellationToken.None);
            if (!bIsCaptchaValid)
                return StatusCode((int) HttpStatusCode.Forbidden, new ApiResponse(HttpMessages.CaptchaInvalid));

            // Submit password change request.
            var forgotPasswordResult = await _userService.RequestPasswordResetAsync(model);

            #region Email broadcast

            var emailTemplate = _emailCacheService.Read(EmailTemplateConstant.ForgotPasswordRequest);

            if (emailTemplate != null)
            {
                await _sendMailService.SendAsync(new HashSet<string> {forgotPasswordResult.Email}, null, null,
                    emailTemplate.Subject,
                    emailTemplate.Content, true, CancellationToken.None);

                _logger.LogInformation(
                    $"Sent message to {forgotPasswordResult.Email} with subject {emailTemplate.Subject}");
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
            if (parameter == null)
            {
                parameter = new SubmitPasswordResetViewModel();
                TryValidateModel(parameter);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);


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
        ///     Using information submitted by user to change account password.
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
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new ApiResponse(HttpMessages.CurrentPasswordIsInvalid));

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
        ///     Change user status by searching for user id.
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
                return StatusCode((int) HttpStatusCode.Forbidden,
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

            //todo: Reason

            // Information has been changed.
            if (bHasInformationChanged)
                await _unitOfWork.CommitAsync();

            #endregion

            #region Real-time message broadcast

            // Send real-time message to all admins.
            var broadcastRealTimeMessageTask = _realTimeService.SendRealTimeMessageToGroupsAsync(
                new[] {RealTimeGroupConstant.Admin}, RealTimeEventConstant.EditUserStatus, user,
                CancellationToken.None);

            // Send push notification to all admin.
            var collapseKey = Guid.NewGuid().ToString("D");
            var realTimeMessage = new RealTimeMessage<User>();
            realTimeMessage.Title = RealTimeMessages.EditUserStatusTitle;
            realTimeMessage.Body = RealTimeMessages.EditUserStatusContent;
            realTimeMessage.AdditionalInfo = user;

            var broadcastPushMessageTask = _realTimeService.SendPushMessageToGroupsAsync(
                new[] {RealTimeGroupConstant.Admin}, collapseKey, realTimeMessage);

            await Task.WhenAll(broadcastRealTimeMessageTask, broadcastPushMessageTask);

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
            if (condition == null)
            {
                condition = new SearchUserViewModel();
                TryValidateModel(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var loadUsersResult = await _userService.SearchUsersAsync(condition);
            return Ok(loadUsersResult);
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
        ///     Request service to send another email to obtain new account activation code.
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
            accounts = accounts.Where(x =>
                x.Email.Equals(info.Email) && x.Status == UserStatus.Pending && x.Type == UserKind.Basic);

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
            {
                _unitOfWork.AccessTokens.Remove(token);
            }
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
                await _sendMailService.SendAsync(new HashSet<string> {account.Email}, null, null, emailTemplate.Subject,
                    emailTemplate.Content, false, CancellationToken.None);

            #endregion

            return Ok();
        }

        /// <summary>
        ///     Initialize account access token.
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
        ///     Initialize user claim.
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
        ///     Instance for accessing database.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;
        
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
        ///     Service which is for handling profile caching.
        /// </summary>
        private readonly IValueCacheService<int, User> _profileCacheService;

        /// <summary>
        ///     Service which is for checking captcha.
        /// </summary>
        private readonly ICaptchaService _captchaService;

        private readonly IRealTimeService _realTimeService;

        private readonly IUserService _userService;

        #endregion
    }
}