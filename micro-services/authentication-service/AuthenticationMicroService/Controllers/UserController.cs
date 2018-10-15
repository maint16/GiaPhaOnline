using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AuthenticationDb.Interfaces;
using AuthenticationDb.Models.Entities;
using AuthenticationMicroService.Constants;
using AuthenticationMicroService.Interfaces.Services;
using AuthenticationMicroService.Interfaces.Services.Businesses;
using AuthenticationMicroService.Models.Jwt;
using AuthenticationMicroService.ViewModels.User;
using AuthenticationShared.Interfaces.Services;
using AuthenticationShared.Models;
using AuthenticationShared.Resources;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AuthenticationMicroService.Controllers
{
    [Route("api/[controller]")]
    public class UserController : ApiBaseController
    {
        #region Properties

        /// <summary>
        ///     Instance for accessing database.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        ///     System time service
        /// </summary>
        private readonly ITimeService _systemTimeService;

        /// <summary>
        ///     Provide access to generic database functions.
        /// </summary>
        private readonly IRelationalDbService _databaseFunction;

        /// <summary>
        ///     Instance which is for accessing identity attached in request.
        /// </summary>
        private readonly IIdentityService _identityService;

        /// <summary>
        ///     Service which is for checking captcha.
        /// </summary>
        private readonly ICaptchaService _captchaService;

        /// <summary>
        ///     Configuration information of JWT.
        /// </summary>
        private readonly JwtConfiguration _jwtConfiguration;

        /// <summary>
        ///     Service which is for handling profile caching.
        /// </summary>
        private readonly IValueCacheService<int, User> _profileCacheService;

        /// <summary>
        ///     Service which is for handling user.
        /// </summary>
        private readonly IUserService _userService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initiate controller with injectors.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="mapper"></param>
        /// <param name="timeService"></param>
        /// <param name="relationalDbService"></param>
        /// <param name="identityService"></param>
        /// <param name="captchaService"></param>
        /// <param name="jwtConfigurationOptions"></param>
        /// <param name="profileCacheService"></param>
        /// <param name="userService"></param>
        public UserController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ITimeService timeService,
            IRelationalDbService relationalDbService,
            IIdentityService identityService,
            ICaptchaService captchaService,
            IOptions<JwtConfiguration> jwtConfigurationOptions,
            IValueCacheService<int, User> profileCacheService,
            IUserService userService) : base(unitOfWork, mapper, timeService, relationalDbService, identityService)
        {
            _unitOfWork = unitOfWork;
            _systemTimeService = timeService;
            _databaseFunction = relationalDbService;
            _identityService = identityService;
            _captchaService = captchaService;
            _jwtConfiguration = jwtConfigurationOptions.Value;
            _profileCacheService = profileCacheService;
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
            var bIsCaptchaValid = await _captchaService.IsCaptchaValidAsync(model.CaptchaCode, null, CancellationToken.None);
            if (!bIsCaptchaValid)
                return StatusCode((int)HttpStatusCode.Forbidden, new ApiResponse());

            var user = await _userService.LoginAsync(model);

            // Initialize jwt token.
            var jwt = InitializeAccountToken(user);
            return Ok(jwt);
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

    }
}
