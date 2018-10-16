using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AppBusiness.Interfaces;
using AppBusiness.Interfaces.Domains;
using AppDb.Interfaces;
using AppDb.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ServiceShared.Exceptions;
using ServiceShared.Interfaces.Services;
using Shared.Enumerations;
using Shared.Enumerations.Order;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.Category;
using SkiaSharp;
using VgySdk.Interfaces;
using VgySdk.Models;

namespace AppBusiness.Domain
{
    public class CategoryDomain : ICategoryDomain
    {
        #region Constructor

        public CategoryDomain(IUnitOfWork unitOfWork, IRelationalDbService relationalDbService,
            ITimeService timeService, IProfileService identityService, IHttpContextAccessor httpContextAccessor, IVgyService vgyService)
        {
            _unitOfWork = unitOfWork;
            _relationalDbService = relationalDbService;
            _timeService = timeService;
            _identityService = identityService;
            _httpContext = httpContextAccessor.HttpContext;
            _vgyService = vgyService;
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


            // Find identity from request.
            var profile = _identityService.GetProfile(_httpContext);

            #region Add category

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

            #endregion


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
        ///     <inheritdoc />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<Category> GetCategoryUsingIdAsync(int id,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var loadCategoryCondition = new SearchCategoryViewModel();
            loadCategoryCondition.Ids = new HashSet<int> { id };
            loadCategoryCondition.Pagination = new Pagination(1, 1);

            return await GetCategories(loadCategoryCondition).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<SearchResult<IList<CategorySummary>>> SearchCategorySummariesAsync(SearchCategorySummaryViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var categorySummaries = GetCategorySummaries(condition);
            var loadCategorySummariesResult = new SearchResult<IList<CategorySummary>>();
            loadCategorySummariesResult.Total = await categorySummaries.CountAsync(cancellationToken);

            // Do pagination.
            categorySummaries = _relationalDbService.Paginate(categorySummaries, condition.Pagination);
            loadCategorySummariesResult.Records = await categorySummaries.ToListAsync(cancellationToken);
            return loadCategorySummariesResult;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="photo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<Category> UploadCategoryPhotoAsync(int categoryId, SKBitmap photo,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Find user.
            var user = await _unitOfWork.Categories.FirstOrDefaultAsync(
                x => x.Id == categoryId && x.Status == ItemStatus.Active, cancellationToken);
            if (user == null)
                throw new ApiException(HttpMessages.AccountIsNotFound, HttpStatusCode.NotFound);

            // Resize image to 512x512 size.
            var resizedSkBitmap = photo.Resize(new SKImageInfo(512, 512), SKBitmapResizeMethod.Lanczos3);

            // Initialize file name.
            var fileName = $"{Guid.NewGuid():D}.png";

            using (var skImage = SKImage.FromBitmap(resizedSkBitmap))
            using (var skData = skImage.Encode(SKEncodedImageFormat.Png, 100))
            using (var memoryStream = new MemoryStream())
            {
                skData.SaveTo(memoryStream);
                var vgySuccessRespone = await _vgyService.UploadAsync<VgySuccessResponse>(memoryStream.ToArray(),
                    "image/png", fileName,
                    CancellationToken.None);

                // Response is empty.
                if (vgySuccessRespone == null || vgySuccessRespone.IsError)
                    throw new ApiException(HttpMessages.ImageIsInvalid, HttpStatusCode.Forbidden);

                user.Photo = vgySuccessRespone.ImageUrl;
            }

            // Save changes into database.
            await _unitOfWork.CommitAsync(cancellationToken);
            return user;
        }

        /// <summary>
        /// Summarize category information asynchronously.
        /// </summary>
        /// <returns></returns>
        public Task SummarizeCategory()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Get categories using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected virtual IQueryable<Category> GetCategories(SearchCategoryViewModel condition)
        {
            // Find identity in request.
            var profile = _identityService.GetProfile(_httpContext);

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

            if (profile != null && profile.Role == UserRole.Admin)
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
            else
            {
                categories = categories.Where(x => x.Status == ItemStatus.Active);
            }

            #endregion

            return categories;
        }

        /// <summary>
        /// Get category summaries using specific condition.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected virtual IQueryable<CategorySummary> GetCategorySummaries(SearchCategorySummaryViewModel condition)
        {
            // Get list of category summaries.
            var categorySummaries = _unitOfWork.CategorySummaries.Search();

            var categoryIds = condition.CategoryIds;
            if (categoryIds != null && categoryIds.Count > 0)
            {
                categoryIds = categoryIds.Where(x => x > 0).ToHashSet();
                if (categoryIds != null && categoryIds.Count > 0)
                    categorySummaries = categorySummaries.Where(x => categoryIds.Contains(x.CategoryId));
            }

            return categorySummaries;
        }

        #endregion

        #region Properties

        private readonly IUnitOfWork _unitOfWork;

        private readonly IRelationalDbService _relationalDbService;

        private readonly ITimeService _timeService;

        private readonly IProfileService _identityService;

        private readonly HttpContext _httpContext;

        private readonly IVgyService _vgyService;

        #endregion
    }
}