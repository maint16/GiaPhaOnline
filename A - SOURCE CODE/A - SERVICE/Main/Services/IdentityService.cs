using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using SystemDatabase.Models.Entities;
using Main.Authentications.ActionFilters;
using Main.Interfaces.Services;
using Main.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Main.Services
{
    public class IdentityService : IIdentityService
    {
        #region Methods

        /// <summary>
        ///     Initiate identity principal by using specific information.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public void SetProfile(HttpContext httpContext, Account account)
        {
            var items = httpContext.Items;
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
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public Account GetProfile(HttpContext httpContext)
        {
            var profile = httpContext.Items[ClaimTypes.Actor];
            if (profile == null)
                return null;

            return (Account)profile;
        }

        /// <summary>
        ///     Initiate jwt from identity.
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="jwtConfiguration"></param>
        /// <returns></returns>
        public string GenerateJwt(Claim[] claims, JwtConfiguration jwtConfiguration)
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
        public T DecodeJwt<T>(string token)
        {
            return default(T);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="authorizationHandlerContext"></param>
        /// <param name="requirement"></param>
        /// <param name="bAnonymousAccessAttributeCheck"></param>
        public void BypassAuthorizationFilter(AuthorizationHandlerContext authorizationHandlerContext, IAuthorizationRequirement requirement, bool bAnonymousAccessAttributeCheck)
        {
            // Anonymous access attribute must be checked.
            if (bAnonymousAccessAttributeCheck)
            {
                // Cast AuthorizationHandlerContext to AuthorizationFilterContext.
                var authorizationFilterContext = (AuthorizationFilterContext)authorizationHandlerContext.Resource;

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