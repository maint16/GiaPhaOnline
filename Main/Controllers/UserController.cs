using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AppBusiness.Interfaces;
using AppBusiness.Interfaces.Domains;
using AppBusiness.Models.Users;
using AppDb.Interfaces;
using AppDb.Models.Entities;
using AppModel.Models;
using AppShared.Resources;
using AppShared.ViewModels.Users;
using AutoMapper;
using ClientShared.Enumerations;
using Main.Constants;
using Main.Constants.RealTime;
using Main.Interfaces.Services;
using Main.Interfaces.Services.RealTime;
using Main.Models.RealTime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceShared.Authentications.ActionFilters;
using ServiceShared.Interfaces.Services;
using ServiceShared.Models;
using SkiaSharp;
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
        /// <param name="profileService"></param>
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
        /// <param name="userDomain"></param>
        public UserController(
            IAppUnitOfWork unitOfWork,
            IMapper mapper,
            ITimeService timeService,
            IBaseRelationalDbService relationalDbService,
            IEncryptionService encryptionService,
            IAppProfileService profileService,
            ITimeService systemTimeService,
            IExternalAuthenticationService externalAuthenticationService,
            ISendMailService sendMailService,
            IEmailCacheService emailCacheService,
            IOptions<AppJwtModel> jwtConfigurationOptions,
            IOptions<ApplicationSetting> applicationSettings,
            ILogger<UserController> logger,
            IVgyService vgyService,
            IValueCacheService<int, User> profileCacheService,
            ICaptchaService captchaService,
            IRealTimeService realTimeService,
            IUserDomain userDomain) : base(
            unitOfWork, mapper, timeService,
            relationalDbService, profileService)
        {
            _logger = logger;
            _profileService = profileService;
            _sendMailService = sendMailService;
            _emailCacheService = emailCacheService;
            _captchaService = captchaService;
            _realTimeService = realTimeService;
            _userDomain = userDomain;
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
            #region Request param validation

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

            #region Captcha validation

            // Verify the captcha.
            var bIsCaptchaValid =
                await _captchaService.IsCaptchaValidAsync(model.CaptchaCode, null, CancellationToken.None);
            if (!bIsCaptchaValid)
                return StatusCode((int) HttpStatusCode.Forbidden, new ApiResponse(HttpMessages.CaptchaInvalid));

            #endregion

            #region Login & token generation

            var user = await _userDomain.LoginAsync(model);
            var jsonWebToken = _userDomain.GenerateJwt(user);

            #endregion

            return Ok(jsonWebToken);
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
            #region Request params validation

            if (info == null)
            {
                info = new GoogleLoginViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Google login

            var user = await _userDomain.GoogleLoginAsync(info);
            var jsonWebToken = _userDomain.GenerateJwt(user);

            #endregion

            return Ok(jsonWebToken);
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
            #region Request parameters validation

            // Information hasn't been initialized.
            if (info == null)
            {
                info = new FacebookLoginViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Facebook login & token generate

            // Do facebook login.
            var user = await _userDomain.FacebookLoginAsync(info);

            // Initialize access token.
            var jsonWebToken = _userDomain.GenerateJwt(user);

            #endregion

            return Ok(jsonWebToken);
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
            var profile = ProfileService.GetProfile();

            var loadUserCondition = new SearchUserViewModel();
            loadUserCondition.Ids = new HashSet<int>();

            if (id == null || id < 1)
                if (profile != null)
                    return Ok(profile);
                else
                    return NotFound();
            loadUserCondition.Ids.Add(id.Value);

            // Only search for active account if user is normal user.
            if (profile != null && profile.Role != UserRole.Admin)
            {
                loadUserCondition.Statuses = new HashSet<UserStatus>();
                loadUserCondition.Statuses.Add(UserStatus.Available);
            }

            // Find the first account in system.
            var user = await _userDomain.SearchUserAsync(loadUserCondition);
            return Ok(user);
        }

        /// <summary>
        ///     Base on specific information to create an account in database.
        /// </summary>
        /// <returns></returns>
        [Route("basic-register")]
        [AllowAnonymous]
        public async Task<IActionResult> BasicRegister([FromBody] RegisterAccountViewModel model)
        {
            #region Request parameters validation

            // Parameters haven't been initialized. Initialize 'em.
            if (model == null)
            {
                model = new RegisterAccountViewModel();
                TryValidateModel(model);
            }

            // Parameters are invalid. Send errors back to client.
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Captcha validation

            // Verify the captcha.
            var bIsCaptchaValid =
                await _captchaService.IsCaptchaValidAsync(model.CaptchaCode, null, CancellationToken.None);
            if (!bIsCaptchaValid)
                return StatusCode((int) HttpStatusCode.Forbidden, new ApiResponse(HttpMessages.CaptchaInvalid));

            #endregion

            #region Basic register

            var basicRegisterResult = await _userDomain.BasicRegisterAsync(model);

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

            #endregion

            #region Send messages to admin

            // Send real-time message to all admins.
            var broadcastRealTimeMessageTask = _realTimeService.SendRealTimeMessageToGroupsAsync(
                new[] {RealTimeGroupConstant.Admin}, RealTimeEventConstant.UserRegistration, basicRegisterResult,
                CancellationToken.None);

            // Send push notification to all admin.
            var collapseKey = Guid.NewGuid().ToString("D");
            var realTimeMessage = new RealTimeMessage<BasicRegisterResultModel>();
            realTimeMessage.Title = RealTimeMessages.AddNewCategoryGroupTitle;
            realTimeMessage.Body = RealTimeMessages.AddNewCategoryGroupContent;
            realTimeMessage.AdditionalInfo = basicRegisterResult;

            var broadcastPushMessageTask = _realTimeService.SendPushMessageToGroupsAsync(
                new[] {RealTimeGroupConstant.Admin}, collapseKey, realTimeMessage);

            await Task.WhenAll(broadcastRealTimeMessageTask, broadcastPushMessageTask);

            #endregion

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
            #region Request parameters validation

            // Parameter hasn't been initialized.
            if (model == null)
            {
                model = new ForgotPasswordViewModel();
                TryValidateModel(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Captcha validation

            // Verify the captcha.
            var bIsCaptchaValid =
                await _captchaService.IsCaptchaValidAsync(model.CaptchaCode, null, CancellationToken.None);
            if (!bIsCaptchaValid)
                return StatusCode((int) HttpStatusCode.Forbidden, new ApiResponse(HttpMessages.CaptchaInvalid));

            #endregion

            #region Business handle

            // Submit password change request.
            var forgotPasswordResult = await _userDomain.RequestPasswordResetAsync(model);

            #endregion

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
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("submit-password-reset")]
        [AllowAnonymous]
        public async Task<IActionResult> SubmitPasswordReset([FromBody] SubmitPasswordResetViewModel model)
        {
            #region Request parameters validation

            if (model == null)
            {
                model = new SubmitPasswordResetViewModel();
                TryValidateModel(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            // Submit password reset.
            var submitPasswordResetResult = await _userDomain.SubmitPasswordResetAsync(model);

            #region Send email

            var emailTemplate = _emailCacheService.Read(EmailTemplateConstant.SubmitPasswordReset);

            if (emailTemplate != null)
                await _sendMailService.SendAsync(new HashSet<string> {model.Email}, null, null, emailTemplate.Subject,
                    emailTemplate.Content, false, CancellationToken.None);

            #endregion

            return Ok();
        }

        /// <summary>
        ///     Using information submitted by user to change account password.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("change-password/{id}")]
        public async Task<IActionResult> ChangePassword([FromRoute] int id, [FromBody] ChangePasswordViewModel model)
        {
            #region Request parameters validation

            if (model == null)
            {
                model = new ChangePasswordViewModel();
                TryValidateModel(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            var profile = _profileService.GetProfile();
            var userId = id < 0 ? profile.Id : id;
            await _userDomain.ChangePasswordAsync(userId, model);

            return Ok();
        }

        /// <summary>
        ///     Change user status by searching for user id.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("status/{id}")]
        [Authorize(Policy = PolicyConstant.IsAdminPolicy)]
        public async Task<IActionResult> ChangeUserStatus([FromRoute] int id,
            [FromBody] ChangeUserStatusViewModel model)
        {
            // Find requester profile.
            var profile = ProfileService.GetProfile();

            // User id is the same as the requester id. This is not allowed because user cannot change his/her account status.
            if (profile.Id == id)
                return StatusCode((int) HttpStatusCode.Forbidden,
                    new ApiResponse(HttpMessages.CannotChangeOwnProfileStatus));

            var user = await _userDomain.ChangeUserStatus(id, model);

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

            // Get request profile.
            var profile = _profileService.GetProfile();

            if (profile == null || profile.Role != UserRole.Admin)
                condition.Statuses = new HashSet<UserStatus> {UserStatus.Available};

            var loadUsersResult = await _userDomain.SearchUsersAsync(condition);
            return Ok(loadUsersResult);
        }

        /// <summary>
        ///     Upload Avatar
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("upload-avatar")]
        //[Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadAvatar(UploadPhotoViewModel model)
        {
            #region Request parameters validation

            if (model == null)
            {
                model = new UploadPhotoViewModel();
                TryValidateModel(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Change user profile photo

            // Get requester profile.
            var profile = ProfileService.GetProfile();
            if (model.UserId == null)
                model.UserId = profile.Id;

            if (model.UserId != profile.Id && profile.Role != UserRole.Admin)
                return StatusCode((int) HttpStatusCode.Forbidden,
                    new ApiResponse(HttpMessages.HasNoPermissionChangeUserProfilePhoto));

            // Reflect image variable.
            var image = model.Photo;

            using (var skManagedStream = new SKManagedStream(image.OpenReadStream()))
            {
                var skBitmap = SKBitmap.Decode(skManagedStream);

                try
                {
                    var user = await _userDomain.UploadUserProfileImageAsync(profile.Id, skBitmap);
                    return Ok(user);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception.Message, exception);
                    return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse(HttpMessages.ImageIsInvalid));
                }
            }

            #endregion
        }

        /// <summary>
        ///     Request service to send another email to obtain new account activation code.
        /// </summary>
        /// <returns></returns>
        [HttpPost("resend-activation-code")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendAccountActivationCode(
            [FromBody] RequestUserActivationCodeViewModel model)
        {
            #region Request parameters validation

            if (model == null)
            {
                model = new RequestUserActivationCodeViewModel();
                TryValidateModel(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            // Generate user activation token.
            var addUserActivationTokenResult = await _userDomain.RequestUserActivationTokenAsync(model);

            #region Send email

            var emailTemplate = _emailCacheService.Read(EmailTemplateConstant.ResendAccountActivationCode);

            if (emailTemplate != null)
                await _sendMailService.SendAsync(new HashSet<string> {addUserActivationTokenResult.Email}, null, null,
                    emailTemplate.Subject,
                    emailTemplate.Content, false, CancellationToken.None);

            #endregion

            return Ok();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Logging instance.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        ///     Instance which is for accessing identity attached in request.
        /// </summary>
        private readonly IAppProfileService _profileService;

        /// <summary>
        ///     Send email service
        /// </summary>
        private readonly ISendMailService _sendMailService;

        /// <summary>
        ///     Email cache service.
        /// </summary>
        private readonly IEmailCacheService _emailCacheService;

        /// <summary>
        ///     Service which is for checking captcha.
        /// </summary>
        private readonly ICaptchaService _captchaService;

        private readonly IRealTimeService _realTimeService;

        private readonly IUserDomain _userDomain;

        #endregion
    }
}