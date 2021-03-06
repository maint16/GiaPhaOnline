﻿using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ClientShared.Enumerations;
using MainBusiness.Interfaces;
using MainDb.Interfaces;
using MainDb.Models.Entities;
using MainMicroService.Authentications.Requirements;
using MainMicroService.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using ServiceShared.Authentications.ActionFilters;
using ServiceShared.Interfaces.Services;

namespace MainMicroService.Authentications.Handlers
{
    public class SolidAccountRequirementHandler : AuthorizationHandler<SolidAccountRequirement>
    {
        #region Constructor

        /// <summary>
        ///     Initiate requirement handler with injectors.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="profileService"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="profileCacheService"></param>
        public SolidAccountRequirementHandler(
            IAppUnitOfWork unitOfWork,
            IAppProfileService profileService, IHttpContextAccessor httpContextAccessor,
            IBaseKeyValueCacheService<int, User> profileCacheService)
        {
            _unitOfWork = unitOfWork;
            _profileService = profileService;
            _httpContextAccessor = httpContextAccessor;
            _profileCacheService = profileCacheService;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Handle requirement asychronously.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            SolidAccountRequirement requirement)
        {
            // Convert authorization filter context into authorization filter context.
            var authorizationFilterContext = (AuthorizationFilterContext) context.Resource;

            //var httpContext = authorizationFilterContext.HttpContext;
            var httpContext = _httpContextAccessor.HttpContext;

            // Find claim identity attached to principal.
            var claimIdentity = (ClaimsIdentity) httpContext.User.Identity;

            // Find id from claims list.
            var id = claimIdentity.Claims.Where(x => x.Type.Equals("Id"))
                .Select(x => x.Value)
                .FirstOrDefault();


            // Id is invalid
            if (string.IsNullOrEmpty(id) || !int.TryParse(id, out var iId))
            {
                if (authorizationFilterContext == null)
                {
                    context.Fail();
                    return;
                }

                // Method or controller authorization can be by passed.
                if (authorizationFilterContext.Filters.Any(x => x is ByPassAuthorizationAttribute))
                {
                    _profileService.BypassAuthorizationFilter(context, requirement);
                    return;
                }

                context.Fail();
                return;
            }

            var account = _profileCacheService.Read(iId);

            // Account is found in cache. By pass authentication.
            if (account != null)
            {
                //ClaimsIdentity(account);
                // Update claim identity.
                _profileService.SetProfile(account);
                context.Succeed(requirement);
                return;
            }

            // Find accounts based on conditions.
            var accounts = _unitOfWork.Accounts.Search();
            accounts = accounts.Where(x =>
                x.Id == iId && x.Status == UserStatus.Available);

            // Find the first matched account in the system.
            account = await accounts.FirstOrDefaultAsync();

            // Account is not found.
            if (account == null)
            {
                // Method or controller authorization can be by passed.
                if (authorizationFilterContext.Filters.Any(x => x is ByPassAuthorizationAttribute))
                {
                    _profileService.BypassAuthorizationFilter(context, requirement);
                    return;
                }

                context.Fail();
                return;
            }

            // Add the newly found account to cache for faster querying.
            _profileCacheService.Add(iId, account, LifeTimeConstant.ProfileCacheLifeTime);
            _profileService.SetProfile(account);
            context.Succeed(requirement);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Provides functions to access to database.
        /// </summary>
        private readonly IAppUnitOfWork _unitOfWork;

        /// <summary>
        ///     Provides functions to access service which handles identity businesses.
        /// </summary>
        private readonly IAppProfileService _profileService;

        /// <summary>
        ///     Context accessor.
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        ///     Service which is for caching user information.
        /// </summary>
        private readonly IBaseKeyValueCacheService<int, User> _profileCacheService;

        #endregion
    }
}