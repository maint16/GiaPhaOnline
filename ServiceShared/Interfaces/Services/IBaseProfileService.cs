using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using ServiceShared.Models;

namespace ServiceShared.Interfaces.Services
{
    /// <summary>
    ///     Service which handles identity businesses.
    /// </summary>
    public interface IBaseProfileService
    {
        #region Methods

        /// <summary>
        ///     Initiate jwt from identity.
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="jwtConfiguration"></param>
        /// <returns></returns>
        string GenerateJwt(Claim[] claims, AppJwtModel jwtConfiguration);

        /// <summary>
        ///     Decode token by using specific information.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        T DecodeJwt<T>(string token);

        /// <summary>
        ///     Allow identity to be parsed and set to both anonymous & authenticated users.
        /// </summary>
        void BypassAuthorizationFilter(AuthorizationHandlerContext authorizationHandlerContext,
            IAuthorizationRequirement requirement, bool bAnonymousAccessAttributeCheck = false);

        #endregion
    }
}