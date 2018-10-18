using System.Security.Claims;
using AppDb.Models.Entities;
using AppModel.Models;
using Microsoft.AspNetCore.Authorization;

namespace AppBusiness.Interfaces
{
    /// <summary>
    ///     Service which handles identity businesses.
    /// </summary>
    public interface IProfileService
    {
        #region Methods

        /// <summary>
        ///     Initiate identity claim from user information.
        /// </summary>
        /// <returns></returns>
        void SetProfile(User account);

        /// <summary>
        ///     Get identity attached in request.
        /// </summary>
        /// <returns></returns>
        User GetProfile();

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