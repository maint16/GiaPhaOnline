using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Main.Interfaces.Services;
using Main.Models;
using Main.ViewModels.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Enumerations;
using Shared.Interfaces.Services;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.Accounts;
using System.Security.Principal;
using SystemDatabase.Enumerations;
using SystemDatabase.Models.Entities;
using Microsoft.Extensions.Logging;

namespace Main.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        #region Constructors

        /// <summary>
        ///     Initiate controller with injectors.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="encryptionService"></param>
        /// <param name="identityService"></param>
        /// <param name="systemTimeService"></param>
        /// <param name="jwtConfigurationOptions"></param>
        /// <param name="applicationSettings"></param>
        /// <param name="logger"></param>
        public AccountController(
            IUnitOfWork unitOfWork,
            IEncryptionService encryptionService,
            IIdentityService identityService,
            ITimeService systemTimeService,
            IOptions<JwtConfiguration> jwtConfigurationOptions,
            IOptions<ApplicationSetting> applicationSettings,
            ILogger<AccountController> logger)
        {
            _unitOfWork = unitOfWork;
            _encryptionService = encryptionService;
            _identityService = identityService;
            _systemTimeService = systemTimeService;
            _jwtConfiguration = jwtConfigurationOptions.Value;
            _applicationSettings = applicationSettings.Value;
            _logger = logger;
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

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

#if !ALLOW_ANONYMOUS
            #region Search account

            // Find account with specific information in database.
            var condition = new SearchAccountViewModel();

            // Email search condition.
            condition.Email = new TextSearch();
            condition.Email.Mode = TextSearchMode.Equal;
            condition.Email.Value = parameters.Email;

            // Find account by password.
            condition.Password = new TextSearch();
            condition.Password.Value = _encryptionService.Md5Hash(parameters.Password);
            condition.Password.Mode = TextSearchMode.EqualIgnoreCase;

            condition.Statuses = new[] { AccountStatus.Active };

            // Find accounts with defined condition above.
            var accounts = _unitOfWork.RepositoryAccounts.Search();
            accounts = _unitOfWork.RepositoryAccounts.Search(accounts, condition);

            // Find the first account in database.
            var account = await accounts.FirstOrDefaultAsync();
            if (account == null)
                return NotFound(new HttpResponse(HttpMessages.AccountIsNotFound));

            #endregion
#else
            var account = new Account();
            account.Email = "redplane_dt@yahoo.com.vn";
            account.Nickname = "Linh Nguyen";
#endif

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
            var jwtSecurityToken = new JwtSecurityToken(_jwtConfiguration.Issuer, _jwtConfiguration.Audience, claims, null, jwtExpiration, _jwtConfiguration.SigningCredentials);

            // Initiate token handler which is for generating token code.
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);

            #region Jwt initialization

            var jwt = new JwtResponse();
            jwt.Code = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);
            jwt.LifeTime = _jwtConfiguration.LifeTime;
            jwt.Expiration = _systemTimeService.DateTimeUtcToUnix(jwtExpiration);

            #endregion

            return Ok(jwt);
        }

        /// <summary>
        ///     Find personal profile.
        /// </summary>
        /// <returns></returns>
        [HttpGet("personal-profile")]
        public IActionResult FindProfile()
        {
            var identity = (ClaimsIdentity)Request.HttpContext.User.Identity;
            var claims = identity.Claims.ToDictionary(x => x.Type, x => x.Value);
            return Ok(claims);
        }

        /// <summary>
        ///     Base on specific information to create an account in database.
        /// </summary>
        /// <returns></returns>
        [Route("register")]
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

            #region Search account

            // Initiate search account condition.
            var condition = new SearchAccountViewModel();
            condition.Email = new TextSearch();
            condition.Email.Mode = TextSearchMode.EqualIgnoreCase;
            condition.Email.Value = parameters.Email;

            // Search accounts.
            var accounts = _unitOfWork.RepositoryAccounts.Search();
            accounts = _unitOfWork.RepositoryAccounts.Search(accounts, condition);

            var account = await accounts.FirstOrDefaultAsync();

            // Account exists in system.
            if (account != null)
            {
                Response.StatusCode = (int)HttpStatusCode.Conflict;
                return Json(new HttpResponse(HttpMessages.AccountIsInUse));
            }

            #endregion

            #region Initiate account

            // Initiate account with specific information.
            account = new Account();
            account.Email = parameters.Email;
            account.Password = _encryptionService.Md5Hash(parameters.Password);

            // Add account into database.
            _unitOfWork.RepositoryAccounts.Insert(account);

            // Save changes asychronously.
            await _unitOfWork.CommitAsync();

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

            #endregion

            #region Email search

            // Initiate search conditions.
            var conditions = new SearchAccountViewModel();
            conditions.Email = new TextSearch(TextSearchMode.EndsWithIgnoreCase, parameter.Email);
            conditions.Statuses = new[] { AccountStatus.Active };

            // Search user in database.
            var accounts = _unitOfWork.RepositoryAccounts.Search();
            var account = await _unitOfWork.RepositoryAccounts.Search(accounts, conditions).FirstOrDefaultAsync();

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
            token.IssuedTime = _systemTimeService.DateTimeUtcToUnix(systemTime);
            token.ExpiredTime = _systemTimeService.DateTimeUtcToUnix(expiration);

            // Save token into database.
            _unitOfWork.RepositoryTokens.Insert(token);

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
            #region Model validation

            if (parameter == null)
            {
                parameter = new SubmitPasswordResetViewModel();
                TryValidateModel(parameter);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Information search

            // Find active accounts.
            var accounts = _unitOfWork.RepositoryAccounts.Search();
            accounts = accounts.Where(x => x.Status == AccountStatus.Active);

            // Find active token.
            var epochSystemTime = _systemTimeService.DateTimeUtcToUnix(DateTime.UtcNow);
            var tokens = _unitOfWork.RepositoryTokens.Search();

            // Find token.
            var result = from account in accounts
                         from token in tokens
                         where account.Id == token.OwnerId && token.ExpiredTime < epochSystemTime
                         select new SearchAccountTokenResult
                         {
                             Token = token,
                             Account = account
                         };

            // No active token is found.
            if (!await result.AnyAsync())
                return NotFound(HttpMessages.InformationNotFound);

            #endregion

            #region Information change

            // Hash the password.
            var password = _encryptionService.Md5Hash(parameter.Password);

            // Delete all found tokens.
            _unitOfWork.RepositoryTokens.Remove(result.Select(x => x.Token));
            await result.ForEachAsync(x => x.Account.Password = password);

            // Commit changes.
            await _unitOfWork.CommitAsync();

            #endregion

            return Ok();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Provides functions & repositories to access database.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        ///     Provides functions to encrypt/decrypt data.
        /// </summary>
        private readonly IEncryptionService _encryptionService;

        /// <summary>
        ///     Service which handles identity businesses.
        /// </summary>
        private readonly IIdentityService _identityService;

        /// <summary>
        ///     Service which handles time on system.
        /// </summary>
        private readonly ITimeService _systemTimeService;

        /// <summary>
        ///     Configuration information of JWT.
        /// </summary>
        private readonly JwtConfiguration _jwtConfiguration;

        /// <summary>
        ///     Collection of settings in application.
        /// </summary>
        private readonly ApplicationSetting _applicationSettings;

        private readonly ILogger _logger;

        #endregion
    }
}