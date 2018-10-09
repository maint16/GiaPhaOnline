﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AppDb.Interfaces;
using AppDb.Models.Entities;
using AppModel.Enumerations;
using AppModel.Enumerations.Order;
using AppModel.Exceptions;
using Main.Interfaces.Services;
using Main.Interfaces.Services.Businesses;
using Main.ViewModels.Category;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Services;
using Shared.Models;
using Shared.Resources;

namespace Main.Services.Businesses
{
    public class CategoryService : ICategoryService
    {
        #region Constructor

        public CategoryService(IUnitOfWork unitOfWork, IRelationalDbService relationalDbService,
            ITimeService timeService, IIdentityService identityService, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _relationalDbService = relationalDbService;
            _timeService = timeService;
            _identityService = identityService;
            _httpContext = httpContextAccessor.HttpContext;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<Category> AddCategoryAsync(AddCategoryViewModel model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Find category.
            var categories = _unitOfWork.Categories.Search();
            categories = categories.Where(x => x.Name == model.Name && x.Status == ItemStatus.Active);

            // Check whether category exists or not.
            var bIsCategoryAvailable = await categories.AnyAsync();
            if (bIsCategoryAvailable)
                throw new ApiException(HttpMessages.CategoryCannotConflict, HttpStatusCode.Conflict);

            #endregion

            // Find identity from request.
            var profile = _identityService.GetProfile(_httpContext);

            // Category intialization.
            var category = new Category();
#if USE_IN_MEMORY
            category.Id = _unitOfWork.Categories.Search().OrderByDescending(x => x.Id).Select(x => x.Id)
                              .FirstOrDefault() + 1;
#endif
            category.CreatorId = profile.Id;
            category.CategoryGroupId = model.CategoryGroupId;
            category.Name = model.Name;
            category.Description = model.Description;
            category.Status = ItemStatus.Active;
            category.CreatedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);
            category.LastModifiedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Insert category into system.
            _unitOfWork.Categories.Insert(category);

            await _unitOfWork.CommitAsync(cancellationToken);
            return category;
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<Category> EditCategoryAsync(int id, EditCategoryViewModel model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Get request identity.
            var profile = _identityService.GetProfile(_httpContext);

            // Get all category in database.
            var categories = _unitOfWork.Categories.Search();

            categories = categories.Where(x => x.Id == id && x.Status == ItemStatus.Active);

            // Get the first matched category group.
            var category = await categories.FirstOrDefaultAsync(cancellationToken);
            if (category == null)
                throw new ApiException(HttpMessages.CategoryNotFound, HttpStatusCode.NotFound);

            // Check whether information has been updated or not.
            var bHasInformationChanged = false;

            // Category group id is defined
            if (model.CategoryGroupId != category.CategoryGroupId)
            {
                category.CategoryGroupId = model.CategoryGroupId;
                bHasInformationChanged = true;
            }

            // Name is defined
            if (model.Name != null && model.Name != category.Name)
            {
                category.Name = model.Name;
                bHasInformationChanged = true;
            }

            // Description is defined
            if (model.Description != null && model.Description != category.Description)
            {
                category.Description = model.Description;
                bHasInformationChanged = true;
            }

            // Status is defined.
            if (model.Status != category.Status)
            {
                category.Status = model.Status;
                bHasInformationChanged = true;
            }

            if (bHasInformationChanged)
            {
                category.LastModifiedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

                // Commit changes to database.
                await _unitOfWork.CommitAsync(cancellationToken);
            }

            return category;
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<SearchResult<IList<Category>>> SearchCategoriesAsync(
            SearchCategoryViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var categories = GetCategories(condition);

            // Sort by properties.
            if (condition.Sort != null)
                categories =
                    _relationalDbService.Sort(categories, condition.Sort.Direction,
                        condition.Sort.Property);
            else
                categories = _relationalDbService.Sort(categories, SortDirection.Decending,
                    CategoriesSort.Name);

            // Result initialization.
            var loadCategoriesResult = new SearchResult<IList<Category>>();
            loadCategoriesResult.Total = await categories.CountAsync(cancellationToken);
            loadCategoriesResult.Records = await _relationalDbService.Paginate(categories, condition.Pagination)
                .ToListAsync(cancellationToken);
            return loadCategoriesResult;
        }

        /// <summary>
        ///     Get categories using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected virtual IQueryable<Category> GetCategories(SearchCategoryViewModel condition)
        {
            // Find identity in request.
            var identity = _identityService.GetProfile(_httpContext);

            #region Search for information

            // Get all category
            var categories = _unitOfWork.Categories.Search();

            // Id have been defined.
            var ids = condition.Ids;
            if (ids != null && ids.Count > 0)
            {
                ids = ids.Where(x => x > 0).ToHashSet();
                if (ids != null && ids.Count > 0)
                    categories = categories.Where(x => ids.Contains(x.Id));
            }

            // Category group id have been defined.
            var categoryGroupIds = condition.CategoryGroupIds;
            if (categoryGroupIds != null && categoryGroupIds.Count > 0)
            {
                categoryGroupIds = categoryGroupIds.Where(x => x > 0).ToHashSet();
                if (categoryGroupIds != null && categoryGroupIds.Count > 0)
                    categories = categories.Where(x => categoryGroupIds.Contains(x.CategoryGroupId));
            }

            // Creator Id have been defined.
            var creatorIds = condition.CreatorIds;
            if (creatorIds != null && creatorIds.Count > 0)
            {
                creatorIds = creatorIds.Where(x => x > 0).ToHashSet();
                if (creatorIds != null && creatorIds.Count > 0)
                    categories = categories.Where(x => creatorIds.Contains(x.CreatorId));
            }

            // Name have been defined.
            var names = condition.Names;
            if (condition.Names != null && condition.Names.Count > 0)
            {
                names = names.Where(x => !string.IsNullOrWhiteSpace(x)).ToHashSet();
                if (names != null && names.Count > 0)
                    categories = categories.Where(x => condition.Names.Any(y => x.Name.Contains(y)));
            }

            // Description have been defined.
            var descriptions = condition.Descriptions;
            if (descriptions != null && descriptions.Count > 0)
            {
                descriptions = descriptions.Where(x => !string.IsNullOrWhiteSpace(x)).ToHashSet();
                if (descriptions != null && descriptions.Count > 0)
                    categories = categories.Where(x => condition.Descriptions.Any(y => x.Description.Contains(y)));
            }

            // Search conditions which are based on roles.

            if (identity?.Role == UserRole.Admin)
            {
                var statuses = condition.Statuses;
                if (statuses != null && statuses.Count > 0)
                {
                    condition.Statuses =
                        statuses.Where(x => Enum.IsDefined(typeof(ItemStatus), x)).ToHashSet();
                    if (statuses.Count > 0)
                        categories = categories.Where(x => condition.Statuses.Contains(x.Status));
                }
            }

            return categories;
        }

        #endregion

        #region Properties

        private readonly IUnitOfWork _unitOfWork;

        private readonly IRelationalDbService _relationalDbService;

        private readonly ITimeService _timeService;

        private readonly IIdentityService _identityService;

        private readonly HttpContext _httpContext;

        #endregion
    }
}