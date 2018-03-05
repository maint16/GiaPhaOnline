﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using SystemConstant.Enumerations;
using SystemConstant.Models;
using SystemDatabase.Interfaces;
using SystemDatabase.Interfaces.Repositories;
using SystemDatabase.Models.Entities;
using AutoMapper;
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
using Shared.ViewModels.Accounts;

namespace Main.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : ApiBaseController
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
        /// <param name="jwtConfigurationOptions"></param>
        /// <param name="applicationSettings"></param>
        /// <param name="logger"></param>
        public AccountController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ITimeService timeService,
            IDbSharedService dbSharedService,
            IEncryptionService encryptionService,
            IIdentityService identityService,
            ITimeService systemTimeService,
            IExternalAuthenticationService externalAuthenticationService,
            IOptions<JwtConfiguration> jwtConfigurationOptions,
            IOptions<ApplicationSetting> applicationSettings,
            ILogger<AccountController> logger) : base(unitOfWork, mapper, timeService, dbSharedService, identityService)
        {
            _encryptionService = encryptionService;
            _jwtConfiguration = jwtConfigurationOptions.Value;
            _applicationSettings = applicationSettings.Value;
            _logger = logger;
            _externalAuthenticationService = externalAuthenticationService;
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
            var tokenInfo =  await _externalAuthenticationService.GetGoogleTokenInfoAsync(info.Code);
            if (tokenInfo == null || string.IsNullOrWhiteSpace(tokenInfo.Id))
                return StatusCode((int) HttpStatusCode.Forbidden, new ApiResponse(HttpMessages.GoogleCodeIsInvalid));

            // Get the profile information.
            var profile = await _externalAuthenticationService.GetGoogleBasicProfileAsync(tokenInfo.Id);
            if (profile == null)
                return StatusCode((int) HttpStatusCode.Forbidden, HttpMessages.GoogleCodeIsInvalid);

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
                    return StatusCode((int) HttpStatusCode.Forbidden, new ApiResponse(HttpMessages.AccountIsPending));

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
        /// <returns></returns>
        [HttpGet("personal-profile")]
        public IActionResult FindProfile()
        {
            var identity = (ClaimsIdentity) Request.HttpContext.User.Identity;
            var claims = identity.Claims.ToDictionary(x => x.Type, x => x.Value);
            return Ok(claims);
        }

        /// <summary>
        ///     Base on specific information to create an account in database.
        /// </summary>
        /// <returns></returns>
        [Route("basic-register")]
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
                Response.StatusCode = (int) HttpStatusCode.Conflict;
                return Json(new ApiResponse(HttpMessages.AccountIsInUse));
            }

            #endregion

            #region Initiate account

            // Initiate account with specific information.
            account = new Account();
            account.Email = parameters.Email;
            account.Password = _encryptionService.Md5Hash(parameters.Password);

            // Add account into database.
            UnitOfWork.Accounts.Insert(account);

            // Save changes asychronously.
            await UnitOfWork.CommitAsync();

            // TODO: Implement instruction email.
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
            var conditions = new SearchAccountViewModel();
            conditions.Email = new TextSearch(TextSearchMode.EndsWithIgnoreCase, parameter.Email);
            conditions.Statuses = new[] {AccountStatus.Available};

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

            //TODO: Send instruction email.

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
            var claims = new List<Claim>();
            claims.Add(new Claim(JwtRegisteredClaimNames.Aud, _jwtConfiguration.Audience));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iss, _jwtConfiguration.Issuer));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, account.Email));
            claims.Add(new Claim(nameof(account.Nickname), account.Nickname));

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

        #endregion
    }
}