﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AppBusiness.Interfaces;
using AppBusiness.Interfaces.Domains;
using AppDb.Interfaces;
using AppDb.Models.Entities;
using AppShared.Resources;
using AppShared.ViewModels.FollowCategory;
using ClientShared.Enumerations;
using ClientShared.Enumerations.Order;
using ClientShared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ServiceShared.Exceptions;
using ServiceShared.Interfaces.Services;

namespace AppBusiness.Domain
{
    public class FollowCategoryDomain : IFollowCategoryDomain
    {
        #region Constructor

        public FollowCategoryDomain(IAppUnitOfWork unitOfWork, IProfileService identityService,
            ITimeService timeService,
            IHttpContextAccessor httpContextAccessor, IBaseRelationalDbService relationalDbService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _timeService = timeService;
            _httpContext = httpContextAccessor.HttpContext;
            _relationalDbService = relationalDbService;
        }

        #endregion

        #region Properties

        private readonly IAppUnitOfWork _unitOfWork;

        private readonly HttpContext _httpContext;

        private readonly IProfileService _identityService;

        private readonly ITimeService _timeService;

        private readonly IBaseRelationalDbService _relationalDbService;

        #endregion

        #region Methods

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<FollowCategory> AddFollowCategoryAsync(AddFollowCategoryViewModel model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            #region Find category

            // Find categories.
            var categories = _unitOfWork.Categories.Search();
            categories = categories.Where(x => x.Id == model.CategoryId && x.Status == ItemStatus.Active);

            // Find the first matched result.
            var category = await categories.FirstOrDefaultAsync(cancellationToken);
            if (category == null)
                throw new ApiException(HttpMessages.CategoryNotFound, HttpStatusCode.NotFound);

            #endregion

            #region Check whether user already followed category or not

            // Find request identity.
            var profile = _identityService.GetProfile();

            // Find follow categories.
            var followCategories = _unitOfWork.FollowingCategories.Search();
            followCategories =
                followCategories.Where(x => x.CategoryId == model.CategoryId && x.FollowerId == profile.Id);
            var followCategory = await followCategories.FirstOrDefaultAsync(cancellationToken);

            #endregion

            // Already followed the category.
            if (followCategory != null)
            {
                followCategory.Status = FollowStatus.Following;
            }
            else
            {
                // Initialize follow category.
                followCategory = new FollowCategory();
                followCategory.FollowerId = profile.Id;
                followCategory.CategoryId = model.CategoryId;
                followCategory.Status = FollowStatus.Following;
                followCategory.CreatedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

                // Insert to system.
                _unitOfWork.FollowingCategories.Insert(followCategory);
            }

            // Commit changes.
            await _unitOfWork.CommitAsync(cancellationToken);
            return followCategory;
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task DeleteFollowCategoryAsync(DeleteFollowCategoryViewModel model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Find request identity.
            var profile = _identityService.GetProfile();

            // Find categories by using specific conditions.
            var followCategories = _unitOfWork.FollowingCategories.Search();
            followCategories =
                followCategories.Where(x => x.CategoryId == model.CategoryId && x.FollowerId == profile.Id);

            // Find the first matched category.
            var followCategory = await followCategories.FirstOrDefaultAsync(cancellationToken);
            if (followCategory == null)
                throw new ApiException(HttpMessages.FollowCategoryNotFound, HttpStatusCode.NotFound);

            // Stop following category.
            followCategory.Status = FollowStatus.Ignore;

            // Save changes.
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<SearchResult<IList<FollowCategory>>> SearchFollowCategoriesAsync(
            SearchFollowCategoryViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var followCategories = GetFollowingCategories(condition);

            // Sort property & direction.
            var sort = condition.Sort;
            if (sort != null)
                followCategories = _relationalDbService.Sort(followCategories, sort.Direction, sort.Property);
            else
                followCategories = _relationalDbService.Sort(followCategories, SortDirection.Decending,
                    FollowCategorySort.CreatedTime);


            var loadFollowCategoryResult = new SearchResult<IList<FollowCategory>>();
            loadFollowCategoryResult.Total = await followCategories.CountAsync(cancellationToken);
            loadFollowCategoryResult.Records = await _relationalDbService
                .Paginate(followCategories, condition.Pagination).ToListAsync(cancellationToken);

            return loadFollowCategoryResult;
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<FollowCategory> SearchFollowCategoryAsync(SearchFollowCategoryViewModel condition,
            CancellationToken cancellationToken)
        {
            return await GetFollowingCategories(condition).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        ///     Get following categories using specific condition.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected virtual IQueryable<FollowCategory> GetFollowingCategories(SearchFollowCategoryViewModel condition)
        {
            // Find identity in request.
            var profile = _identityService.GetProfile();

            // Search for posts.
            var followCategories = _unitOfWork.FollowingCategories.Search();

            // Category id is defined.
            var categoryIds = condition.CategoryIds;
            if (categoryIds != null && categoryIds.Count > 0)
            {
                categoryIds = categoryIds.Where(x => x > 0).ToHashSet();
                if (categoryIds.Count > 0)
                    followCategories = followCategories.Where(x => categoryIds.Contains(x.CategoryId));
            }

            // Search conditions which are based on roles.
            if (profile != null && profile.Role == UserRole.Admin)
            {
                // Follower id is defined.
                var followerIds = condition.FollowerIds;
                if (followerIds != null && followerIds.Count > 0)
                {
                    followerIds = followerIds.Where(x => x > 0).ToHashSet();
                    if (followerIds.Count > 0)
                        followCategories = followCategories.Where(x => followerIds.Contains(x.FollowerId));
                }

                // Statuses have been defined.
                var statuses = condition.Statuses;
                if (statuses != null && statuses.Count > 0)
                {
                    statuses =
                        statuses.Where(x => Enum.IsDefined(typeof(ItemStatus), x)).ToHashSet();
                    if (statuses.Count > 0)
                        followCategories = followCategories.Where(x => statuses.Contains(x.Status));
                }
            }
            else
            {
                // Normal users can his/her followed categories.
                followCategories = followCategories.Where(x => x.FollowerId == profile.Id);
            }

            // Created time has been defined.
            var createdTime = condition.CreatedTime;
            if (createdTime != null)
            {
                var from = createdTime.From;
                var to = createdTime.To;

                if (from != null)
                    followCategories = _relationalDbService.SearchNumericProperty(followCategories, x => x.CreatedTime,
                        from.Value,
                        NumericComparision.GreaterEqual);

                if (to != null)
                    followCategories = _relationalDbService.SearchNumericProperty(followCategories, x => x.CreatedTime,
                        to.Value,
                        NumericComparision.LowerEqual);
            }

            return followCategories;
        }

        #endregion
    }
}