using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SystemDatabase.Models.Entities;
using Main.Authentications.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Main.Authentications.Handlers
{
    public class RoleRequirementHandler : AuthorizationHandler<RoleRequirement>
    {
        #region Properties

        /// <summary>
        /// Accessor which is used for accessing into HttpContext.
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region Constructor

        public RoleRequirementHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle requirement asynchronously.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {
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
                context.Fail();
                return Task.CompletedTask;
            }

            // Find account validity.
            var account = (Account) httpContext.Items[ClaimTypes.Actor];
            if (account == null || !requirement.Roles.Contains(account.Role))
            {
                context.Fail();
                return Task.CompletedTask; ;
            }
            
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        #endregion
    }
}