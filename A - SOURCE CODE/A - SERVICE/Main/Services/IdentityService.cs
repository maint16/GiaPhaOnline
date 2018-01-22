using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SystemDatabase.Models.Entities;
using Main.Interfaces.Services;
using Main.Models;
using Microsoft.AspNetCore.Http;

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

        #endregion
    }
}