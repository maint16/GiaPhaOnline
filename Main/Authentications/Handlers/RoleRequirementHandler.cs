using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AppDb.Models.Entities;
using Main.Authentications.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceShared.Authentications.ActionFilters;

namespace Main.Authentications.Handlers
{
    public class RoleRequirementHandler : AuthorizationHandler<RoleRequirement>
    {
        #region Properties

        /// <summary>
        ///     Accessor which is used for accessing into HttpContext.
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initialize role requirement handler with injector.
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public RoleRequirementHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Handle requirement asynchronously.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {
            // Convert authorization filter context into authorization filter context.
            var authorizationFilterContext = (AuthorizationFilterContext) context.Resource;

            // Check context solidity.
            if (_httpContextAccessor == null || _httpContextAccessor.HttpContext == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            // Find HttpContext.
            var httpContext = _httpContextAccessor.HttpContext;

            // Find account which has been embeded into HttpContext.
            if (!httpContext.Items.ContainsKey(ClaimTypes.Actor))
            {
                // Controller or method allow by pass information analyze.
                if (IsAbleToByPass(authorizationFilterContext))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }

                context.Fail();
                return Task.CompletedTask;
            }

            // Find account validity.
            var account = (User) httpContext.Items[ClaimTypes.Actor];
            if (account == null || !requirement.Roles.Contains(account.Role))
            {
                // Controller or method allow by pass information analyze.
                if (IsAbleToByPass(authorizationFilterContext))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }

                context.Fail();
                return Task.CompletedTask;
                ;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Whether controller or method can be by passed.
        /// </summary>
        /// <param name="authorizationFilterContext"></param>
        /// <returns></returns>
        private bool IsAbleToByPass(AuthorizationFilterContext authorizationFilterContext)
        {
            return authorizationFilterContext.Filters.Any(x => x is ByPassAuthorizationAttribute);
        }

        #endregion
    }
}