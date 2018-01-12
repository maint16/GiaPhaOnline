using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using SystemDatabase.Models.Entities;
using Main.Interfaces.Services;
using Main.Models;

namespace Main.Services
{
    public class IdentityService : IIdentityService
    {
        #region Methods

        /// <summary>
        ///     Initiate identity principal by using specific information.
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public IIdentity InitiateIdentity(Account account)
        {
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimTypes.Email, account.Email));
            identity.AddClaim(new Claim(ClaimTypes.Name, account.Nickname));
            return identity;
        }

        /// <summary>
        ///     Initiate jwt from identity.
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="jwtConfiguration"></param>
        /// <returns></returns>
        public string InitiateToken(Claim[] claims, JwtConfiguration jwtConfiguration)
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
        /// Decode token by using specific information.
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