using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AuthenticationBusiness.Interfaces.Domains;
using AuthenticationDb.Models.Entities;
using AuthenticationMicroService.Interfaces.Services;
using AuthenticationShared.Resources;
using AuthenticationShared.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceShared.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AuthenticationMicroService.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        #region Constructors

        /// <summary>
        ///     Initiate controller with injectors.
        /// </summary>
        /// <param name="captchaService"></param>
        /// <param name="profileCacheService"></param>
        /// <param name="userDomain"></param>
        public UserController(
            ICaptchaService captchaService,
            IValueCacheService<int, User> profileCacheService,
            IUserDomain userDomain)
        {
            _captchaService = captchaService;
            _userDomain = userDomain;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Service which is for checking captcha.
        /// </summary>
        private readonly ICaptchaService _captchaService;

        /// <summary>
        ///     Service which is for handling user.
        /// </summary>
        private readonly IUserDomain _userDomain;

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
        public async Task<IActionResult> BasicLogin([FromBody] LoginViewModel model)
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

            var user = await _userDomain.LoginAsync(model);

            // Initialize jwt token.
            var jsonWebToken = _userDomain.GenerateJwt(user);

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

        #endregion
    }
}