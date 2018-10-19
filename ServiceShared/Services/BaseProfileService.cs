using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using ServiceShared.Authentications.ActionFilters;
using ServiceShared.Interfaces.Services;
using ServiceShared.Models;

namespace ServiceShared.Services
{
    public class BaseProfileService : IBaseProfileService
    {
        #region Constructors

        public BaseProfileService(IOptions<AppJwtModel> appJwt, IHttpContextAccessor httpContextAccessor)
        {
            _appJwt = appJwt.Value;
            _httpContext = httpContextAccessor.HttpContext;
        }

        #endregion

        #region Properties

        private readonly AppJwtModel _appJwt;

        private readonly HttpContext _httpContext;

        #endregion

        #region Methods

        /// <summary>
        ///     Initiate identity principal by using specific information.
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        protected virtual void SetProfile<T>(T account)
        {
            var items = _httpContext.Items;
            if (items.ContainsKey(ClaimTypes.Actor))
            {
                items[ClaimTypes.Actor] = account;
                return;
            }

            items.Add(ClaimTypes.Actor, account);
        }

        /// <summary>
        ///     Get identity attached in request.
        /// </summary>
        /// <returns></returns>
        protected virtual T GetProfile<T>()
        {
            var profile = _httpContext.Items[ClaimTypes.Actor];
            if (profile == null)
                return default(T);

            return (T) profile;
        }

        /// <summary>
        ///     <inheritdoc cref="" />
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="jwtConfiguration"></param>
        /// <returns></returns>
        public virtual string GenerateJwt(Claim[] claims, AppJwtModel jwtConfiguration)
        {
            var systemTime = DateTime.Now;
            var expiration = systemTime.AddSeconds(jwtConfiguration.LifeTime);

            // Create the JWT and write it to a string
            var jwt = new JwtSecurityToken(jwtConfiguration.Issuer, jwtConfiguration.Audience, claims, systemTime,
                expiration, jwtConfiguration.SigningCredentials);

            // From specific information, write token.
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        /// <summary>
        ///     Decode token by using specific information.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual T DecodeJwt<T>(string token)
        {
            return default(T);
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="authorizationHandlerContext"></param>
        /// <param name="requirement"></param>
        /// <param name="bAnonymousAccessAttributeCheck"></param>
        public virtual void BypassAuthorizationFilter(AuthorizationHandlerContext authorizationHandlerContext,
            IAuthorizationRequirement requirement, bool bAnonymousAccessAttributeCheck)
        {
            // Anonymous access attribute must be checked.
            if (bAnonymousAccessAttributeCheck)
            {
                // Cast AuthorizationHandlerContext to AuthorizationFilterContext.
                var authorizationFilterContext = (AuthorizationFilterContext) authorizationHandlerContext.Resource;

                // No allow anonymous attribute has been found.
                if (!authorizationFilterContext.Filters.Any(x => x is ByPassAuthorizationAttribute))
                    return;
            }

            // User doesn't have primary identity.
            if (authorizationHandlerContext.User.Identities.All(x => x.Name != "Anonymous"))
                authorizationHandlerContext.User.AddIdentity(new GenericIdentity("Anonymous"));
            authorizationHandlerContext.Succeed(requirement);
        }

        #endregion
    }
}