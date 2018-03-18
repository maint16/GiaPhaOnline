using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SystemConstant.Enumerations;
using SystemConstant.Models;
using SystemDatabase.Interfaces;
using Main.Authentications.Requirements;
using Main.Interfaces.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Shared.ViewModels.Accounts;

namespace Main.Authentications.Handlers
{
    public class SolidAccountRequirementHandler : AuthorizationHandler<SolidAccountRequirement>
    {
        #region Constructor

        /// <summary>
        ///     Initiate requirement handler with injectors.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="identityService"></param>
        /// <param name="httpContextAccessor"></param>
        public SolidAccountRequirementHandler(
            IUnitOfWork unitOfWork,
            IIdentityService identityService, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion
        
        #region Methods

        /// <summary>
        ///     Handle requirement asychronously.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            SolidAccountRequirement requirement)
        {
            // Convert authorization filter context into authorization filter context.
            var authorizationFilterContext = (AuthorizationFilterContext) context.Resource;

            //var httpContext = authorizationFilterContext.HttpContext;
            var httpContext = _httpContextAccessor.HttpContext;

            // Find claim identity attached to principal.
            var claimIdentity = (ClaimsIdentity) httpContext.User.Identity;

            // Find email from claims list.
            var email =
                claimIdentity.Claims.Where(x => x.Type.Equals(ClaimTypes.Email))
                    .Select(x => x.Value)
                    .FirstOrDefault();

            // Email is invalid.
            if (string.IsNullOrEmpty(email))
            {
                context.Fail();
                return;
            }
            
            // Find accounts based on conditions.
            var accounts = _unitOfWork.Accounts.Search();
            accounts = accounts.Where(x =>
                x.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase) && x.Status == AccountStatus.Available);

            // Find the first matched account in the system.
            var account = await accounts.FirstOrDefaultAsync();

            // Account is not found.
            if (account == null)
                return;

            // Initiate claim identity with newer information from database.
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, email));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, account.Nickname));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, Enum.GetName(typeof(AccountRole), account.Role)));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Authentication,
                Enum.GetName(typeof(AccountStatus), account.Status)));

            // Update claim identity.
            _identityService.SetProfile(httpContext, account);
            context.Succeed(requirement);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Provides functions to access to database.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        ///     Provides functions to access service which handles identity businesses.
        /// </summary>
        private readonly IIdentityService _identityService;

        /// <summary>
        ///     Context accessor.
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion
    }
}