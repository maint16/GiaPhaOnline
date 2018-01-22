using System.Security.Claims;
using System.Security.Principal;
using SystemDatabase.Models.Entities;
using Main.Models;
using Microsoft.AspNetCore.Http;

namespace Main.Interfaces.Services
{
    /// <summary>
    /// Service which handles identity businesses.
    /// </summary>
    public interface IIdentityService
    {
        #region Methods

        /// <summary>
        /// Initiate identity claim from user information.
        /// </summary>
        /// <returns></returns>
        void SetProfile(HttpContext httpContext, Account account);

        /// <summary>
        /// Get identity attached in request.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        Account GetProfile(HttpContext httpContext);
        
        /// <summary>
        /// Initiate jwt from identity.
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="jwtConfiguration"></param>
        /// <returns></returns>
        string GenerateJwt(Claim[] claims, JwtConfiguration jwtConfiguration);

        /// <summary>
        /// Decode token by using specific information.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        T DecodeJwt<T>(string token);

        #endregion
    }
}