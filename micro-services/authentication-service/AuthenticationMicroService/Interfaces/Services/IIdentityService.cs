﻿using System.Security.Claims;
using AuthenticationDb.Models.Entities;
using AuthenticationMicroService.Models.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace AuthenticationMicroService.Interfaces.Services
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
        void SetProfile(HttpContext httpContext, User account);

        /// <summary>
        /// Get identity attached in request.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        User GetProfile(HttpContext httpContext);

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

        /// <summary>
        /// Allow identity to be parsed and set to both anonymous & authenticated users.
        /// </summary>
        void BypassAuthorizationFilter(AuthorizationHandlerContext authorizationHandlerContext, IAuthorizationRequirement requirement, bool bAnonymousAccessAttributeCheck = false);

        #endregion
    }
}