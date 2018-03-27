using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using SystemConstant.Enumerations;
using SystemConstant.Models;
using SystemDatabase.Interfaces;
using SystemDatabase.Models.Entities;
using Main.Authentications.ActionFilters;
using Main.Authentications.Requirements;
using Main.Constants;
using Main.Interfaces.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.OData;
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
        /// <param name="profileCacheService"></param>
        public SolidAccountRequirementHandler(
            IUnitOfWork unitOfWork,
            IIdentityService identityService, IHttpContextAccessor httpContextAccessor,
            IValueCacheService<int, Account> profileCacheService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _httpContextAccessor = httpContextAccessor;
            _profileCacheService = profileCacheService;
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
            var authorizationFilterContext = (AuthorizationFilterContext)context.Resource;

            //var httpContext = authorizationFilterContext.HttpContext;
            var httpContext = _httpContextAccessor.HttpContext;

            // Find claim identity attached to principal.
            var claimIdentity = (ClaimsIdentity)httpContext.User.Identity;

            // Find id from claims list.
            var id = claimIdentity.Claims.Where(x => x.Type.Equals("Id"))
                .Select(x => x.Value)
                .FirstOrDefault();


            // Id is invalid
            if (string.IsNullOrEmpty(id) || !int.TryParse(id, out int iId))
            {
                // Method or controller authorization can be by passed.
                if (authorizationFilterContext.Filters.Any(x => x is ByPassAuthorizationAttribute))
                {
                    _identityService.BypassAuthorizationFilter(context, requirement);
                    return;
                }

                context.Fail();
                return;
            }

            var account = _profileCacheService.Read(iId);

            // Account is found in cache. By pass authentication.
            if (account != null)
            {
                //ClaimsIdentity(account);
                // Update claim identity.
                _identityService.SetProfile(httpContext, account);
                context.Succeed(requirement);
                return;
            }

            // Find accounts based on conditions.
            var accounts = _unitOfWork.Accounts.Search();
            accounts = accounts.Where(x =>
                x.Id == iId && x.Status == AccountStatus.Available);

            // Find the first matched account in the system.
            account = await accounts.FirstOrDefaultAsync();

            // Account is not found.
            if (account == null)
            {
                // Method or controller authorization can be by passed.
                if (authorizationFilterContext.Filters.Any(x => x is ByPassAuthorizationAttribute))
                {
                    _identityService.BypassAuthorizationFilter(context, requirement);
                    return;
                }

                context.Fail();
                return;
            }

            //// Initiate claim identity with newer information from database.
            //var claimsIdentity = new ClaimsIdentity();
            //claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, account.Email));
            //claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, account.Nickname));
            //claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, Enum.GetName(typeof(AccountRole), account.Role)));
            //claimsIdentity.AddClaim(new Claim(ClaimTypes.Authentication,
            //    Enum.GetName(typeof(AccountStatus), account.Status)));
            _profileCacheService.Add(iId, account, LifeTimeConstant.ProfileCacheLifeTime);
            _identityService.SetProfile(httpContext, account);
            context.Succeed(requirement);
        }

        //private ClaimsIdentity ClaimsIdentity(Account account)
        //{
        //    // Initiate claim identity with newer information from database.
        //    var claimsIdentity = new ClaimsIdentity();
        //    claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, account.Email));
        //    claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, account.Nickname));
        //    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, Enum.GetName(typeof(AccountRole), account.Role)));
        //    claimsIdentity.AddClaim(new Claim(ClaimTypes.Authentication,
        //        Enum.GetName(typeof(AccountStatus), account.Status)));
        //}

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

        /// <summary>
        /// 
        /// </summary>
        private readonly IValueCacheService<int, Account> _profileCacheService;

        #endregion
    }
}