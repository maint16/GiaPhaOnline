using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AppBusiness.Interfaces;
using AppBusiness.Interfaces.Domains;
using AppDb.Interfaces;
using AppDb.Models.Entities;
using AppModel.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Enumerations;
using Shared.Enumerations.Order;
using Shared.Interfaces.Services;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.CategoryGroup;

namespace AppBusiness.Domain
{
    public class CategoryGroupDomain : ICategoryGroupDomain
    {
        #region Constructor

        public CategoryGroupDomain(IProfileService identityService, ITimeService timeService,
            IRelationalDbService relationalDbService, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _identityService = identityService;
            _timeService = timeService;
            _relationalDbService = relationalDbService;
            _unitOfWork = unitOfWork;
            _httpContext = httpContextAccessor.HttpContext;
        }

        #endregion

        #region Properties

        private readonly IProfileService _identityService;

        private readonly ITimeService _timeService;

        private readonly IRelationalDbService _relationalDbService;

        private readonly IUnitOfWork _unitOfWork;

        private readonly HttpContext _httpContext;

        #endregion

        #region Methods

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<CategoryGroup> AddCategoryGroup(AddCategoryGroupViewModel model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Find category group.
            var categoryGroups = _unitOfWork.CategoryGroups.Search();
            categoryGroups =
                categoryGroups.Where(x => x.Name == model.Name && x.Status == ItemStatus.Active);

            // Check whether category group exists or not.
            var categoryGroup = await categoryGroups.FirstOrDefaultAsync(cancellationToken);
            if (categoryGroup != null)
                throw new ApiException(HttpMessages.CategoryGroupCannotConflict, HttpStatusCode.Conflict);

            // Find identity from request.
            var profile = _identityService.GetProfile(_httpContext);

            // Category group intialization.
            categoryGroup = new CategoryGroup();

#if USE_IN_MEMORY
            categoryGroup.Id =
                _unitOfWork.CategoryGroups.Search().OrderByDescending(x => x.Id).Select(x => x.Id).FirstOrDefault() + 1;
#endif

            categoryGroup.CreatorId = profile.Id;
            categoryGroup.Name = model.Name;
            categoryGroup.Description = model.Description;
            categoryGroup.Status = ItemStatus.Active;
            categoryGroup.CreatedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);
            categoryGroup.LastModifiedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Insert category group into system.
            _unitOfWork.CategoryGroups.Insert(categoryGroup);

            // Save the category group first.
            await _unitOfWork.CommitAsync(cancellationToken);
            return categoryGroup;
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<CategoryGroup> EditCategoryGroup(int id, EditCategoryGroupViewModel model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Get all category group in database.
            var categoryGroups = _unitOfWork.CategoryGroups.Search();

            categoryGroups = categoryGroups.Where(x => x.Id == id && x.Status == ItemStatus.Active);

            // Get the first matched category group.
            var categoryGroup = await categoryGroups.FirstOrDefaultAsync(cancellationToken);
            if (categoryGroup == null)
                throw new ApiException(HttpMessages.CategoryGroupNotFound, HttpStatusCode.NotFound);

            // Check whether information has been updated or not.
            var bHasInformationChanged = false;

            // Name is defined
            var name = model.Name;
            if (name != null && model.Name != categoryGroup.Name)
            {
                categoryGroup.Name = name;
                bHasInformationChanged = true;
            }

            // Description is defined
            var description = model.Description;
            if (description != null && description != categoryGroup.Description)
            {
                categoryGroup.Description = description;
                bHasInformationChanged = true;
            }

            // Status is defined.
            var status = model.Status;
            if (status != categoryGroup.Status)
            {
                categoryGroup.Status = status;
                bHasInformationChanged = true;
            }

            if (!bHasInformationChanged)
                throw new NotModifiedException();

            categoryGroup.LastModifiedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);
            await _unitOfWork.CommitAsync(cancellationToken);
            return categoryGroup;
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<SearchResult<IList<CategoryGroup>>> SearchCategoryGroupsAsync(
            SearchCategoryGroupViewModel conditions,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var categoryGroups = GetCategoryGroups(conditions);

            // Sort by properties.
            if (conditions.Sort != null)
                categoryGroups =
                    _relationalDbService.Sort(categoryGroups, conditions.Sort.Direction,
                        conditions.Sort.Property);
            else
                categoryGroups = _relationalDbService.Sort(categoryGroups, SortDirection.Decending,
                    CategoryGroupSort.Name);

            // Result initialization.
            var loadCategoryGroupsResult = new SearchResult<IList<CategoryGroup>>();
            loadCategoryGroupsResult.Total = await categoryGroups.CountAsync(cancellationToken);
            loadCategoryGroupsResult.Records = await _relationalDbService
                .Paginate(categoryGroups, conditions.Pagination).ToListAsync(cancellationToken);
            return loadCategoryGroupsResult;
        }

        /// <summary>
        ///     Get category groups using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected virtual IQueryable<CategoryGroup> GetCategoryGroups(
            SearchCategoryGroupViewModel condition)
        {
            // Find identity in request.
            var identity = _identityService.GetProfile(_httpContext);

            // Get all category groups
            var categoryGroups = _unitOfWork.CategoryGroups.Search();

            // Id have been defined.
            var ids = condition.Ids;
            if (ids != null && ids.Count > 0)
            {
                ids = ids.Where(x => x > 0).ToHashSet();
                if (ids != null && ids.Count > 0)
                    categoryGroups = categoryGroups.Where(x => ids.Contains(x.Id));
            }

            // Creator id have been defined.
            var creatorIds = condition.CreatorIds;
            if (creatorIds != null && creatorIds.Count > 0)
            {
                creatorIds = creatorIds.Where(x => x > 0).ToHashSet();
                if (creatorIds != null && creatorIds.Count > 0)
                    categoryGroups = categoryGroups.Where(x => creatorIds.Contains(x.CreatorId));
            }

            // Name have been defined.
            var names = condition.Names;
            if (names != null && names.Count > 0)
            {
                names = names.Where(x => !string.IsNullOrWhiteSpace(x)).ToHashSet();
                if (names != null && names.Count > 0)
                    categoryGroups =
                        categoryGroups.Where(x => names.Any(y => x.Name.Contains(y)));
            }

            // Description have been defined.
            var descriptions = condition.Descriptions;
            if (descriptions != null && descriptions.Count > 0)
            {
                descriptions = descriptions.Where(x => !string.IsNullOrWhiteSpace(x)).ToHashSet();
                if (descriptions != null && descriptions.Count > 0)
                    categoryGroups = categoryGroups.Where(x => descriptions.Any(y => x.Description.Contains(y)));
            }

            // Search conditions which are based on roles.
            if (identity?.Role == UserRole.Admin)
            {
                // Statuses have been defined.
                var statuses = condition.Statuses;
                if (statuses != null && statuses.Count > 0)
                {
                    statuses = statuses.Where(x => Enum.IsDefined(typeof(ItemStatus), x)).ToHashSet();
                    if (statuses.Count > 0)
                        categoryGroups = categoryGroups.Where(x => statuses.Contains(x.Status));
                }
            }
            else
            {
                categoryGroups = categoryGroups.Where(x => x.Status == ItemStatus.Active);
            }

            return categoryGroups;
        }

        #endregion
    }
}