using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDb.Interfaces;
using AppDb.Models.Entities;
using AppModel.Enumerations;
using AppModel.Enumerations.Order;
using AutoMapper;
using Main.Interfaces.Services;
using Main.ViewModels.Category;
using Main.ViewModels.CategoryGroup;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Services;
using Shared.Models;
using Shared.Resources;

namespace Main.Controllers
{
    [Route("api/[controller]")]
    public class CategoryController : ApiBaseController
    {
        #region Constructors

        public CategoryController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ITimeService timeService,
            IRelationalDbService relationalDbService,
            IEncryptionService encryptionService,
            IIdentityService identityService) : base(unitOfWork, mapper, timeService,
            relationalDbService, identityService)
        {
            _unitOfWork = unitOfWork;
            _databaseFunction = relationalDbService;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Instance for accessing database.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        ///     Provide access to generic database functions.
        /// </summary>
        private readonly IRelationalDbService _databaseFunction;

        #endregion

        #region Methods

        /// <summary>
        ///     Add category to system.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<IActionResult> AddCategory([FromBody] AddCategoryViewModel info)
        {
            #region Parameters validation

            if (info == null)
            {
                info = new AddCategoryViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Find category

            // Find category.
            var categories = UnitOfWork.Categories.Search();
            categories = categories.Where(x => x.Name == info.Name && x.Status == ItemStatus.Active);

            // Check whether category exists or not.
            var bIsCategoryAvailable = await categories.AnyAsync();
            if (!bIsCategoryAvailable)
                return Conflict(new ApiResponse(HttpMessages.CategoryCannotConflict));

            #endregion

            #region Category initialization

            // Find identity from request.
            var identity = IdentityService.GetProfile(HttpContext);

            // Category intialization.
            var category = new Category();
            category.CreatorId = identity.Id;
            category.CategoryGroupId = info.CategoryGroupId;
            category.Name = info.Name;
            category.Description = info.Description;
            category.Status = ItemStatus.Active;
            category.CreatedTime = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);
            category.LastModifiedTime = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Insert category into system.
            UnitOfWork.Categories.Insert(category);

            await UnitOfWork.CommitAsync();

            #endregion

            return Ok(category);
        }

        /// <summary>
        /// Edit category by using specific information.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCategory([FromRoute] int id, [FromBody] EditCategoryViewModel info)
        {
            #region Parameters validation

            if (info == null)
            {
                info = new EditCategoryViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Find category

            // Get request identity.
            var identity = IdentityService.GetProfile(HttpContext);

            // Get all category in database.
            var categories = UnitOfWork.Categories.Search();

            categories = categories.Where(x => x.Id == id && x.Status == ItemStatus.Active);

            // Get the first matched category group.
            var category = await categories.FirstOrDefaultAsync();
            if (category == null)
                return NotFound(new ApiResponse(HttpMessages.CategoryNotFound));

            #endregion

            #region Update category information

            // Check whether information has been updated or not.
            var bHasInformationChanged = false;

            // Category group id is defined
            if (info.CategoryGroupId != category.CategoryGroupId)
            {
                category.CategoryGroupId = info.CategoryGroupId;
                bHasInformationChanged = true;
            }

            // Name is defined
            if (info.Name != null && info.Name != category.Name)
            {
                category.Name = info.Name;
                bHasInformationChanged = true;
            }

            // Description is defined
            if (info.Description != null && info.Description != category.Description)
            {
                category.Description = info.Description;
                bHasInformationChanged = true;
            }

            // Status is defined.
            if (info.Status != category.Status)
            {
                category.Status = info.Status;
                bHasInformationChanged = true;
            }

            if (bHasInformationChanged)
            {
                category.LastModifiedTime = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);

                // Commit changes to database.
                await UnitOfWork.CommitAsync();
            }

            #endregion

            return Ok(category);
        }

        /// <summary>
        ///     Load category by using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("search")]
        public async Task<IActionResult> LoadCategories([FromBody] SearchCategoryViewModel condition)
        {
            #region Parameters validation

            if (condition == null)
            {
                condition = new SearchCategoryViewModel();
                TryValidateModel(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            // Find identity in request.
            var identity = IdentityService.GetProfile(HttpContext);

            #region Search for information

            // Get all category
            var categories = _unitOfWork.Categories.Search();

            // Id have been defined.
            if (condition.Ids != null && condition.Ids.Count > 0)
            {
                condition.Ids = condition.Ids.Where(x => x > 0).ToList();
                if (condition.Ids != null && condition.Ids.Count > 0)
                {
                    categories = categories.Where(x => condition.Ids.Contains(x.Id));
                }
            }

            // Category group Id have been defined.
            if (condition.CategoryGroupIds != null && condition.CategoryGroupIds.Count > 0)
            {
                condition.CategoryGroupIds = condition.CategoryGroupIds.Where(x => x > 0).ToList();
                if (condition.CategoryGroupIds != null && condition.CategoryGroupIds.Count > 0)
                {
                    categories = categories.Where(x => condition.CategoryGroupIds.Contains(x.CategoryGroupId));
                }
            }

            // Creator Id have been defined.
            if (condition.CreatorIds != null && condition.CreatorIds.Count > 0)
            {
                condition.CreatorIds = condition.CreatorIds.Where(x => x > 0).ToList();
                if (condition.CreatorIds != null && condition.CreatorIds.Count > 0)
                {
                    categories = categories.Where(x => condition.CreatorIds.Contains(x.CreatorId));
                }
            }

            // Name have been defined.
            if (condition.Names != null && condition.Names.Count > 0)
            {
                condition.Names = condition.Names.Where(x => !string.IsNullOrEmpty(x)).ToList();
                if (condition.Names != null && condition.Names.Count > 0)
                {
                    categories = categories.Where(x => condition.Names.Any(y => x.Name.Contains(y)));
                }
            }

            // Description have been defined.
            if (condition.Descriptions != null && condition.Descriptions.Count > 0)
            {
                condition.Descriptions = condition.Descriptions.Where(x => !string.IsNullOrEmpty(x)).ToList();
                if (condition.Descriptions != null && condition.Descriptions.Count > 0)
                {
                    categories = categories.Where(x => condition.Descriptions.Any(y => x.Description.Contains(y)));
                }
            }

            // Search conditions which are based on roles.

            if (identity?.Role == UserRole.Admin)
            {
                // Statuses have been defined.
                if (condition.Statuses != null && condition.Statuses.Count > 0)
                {
                    condition.Statuses =
                        condition.Statuses.Where(x => Enum.IsDefined(typeof(ItemStatus), x)).ToList();
                    if (condition.Statuses.Count > 0)
                        categories = categories.Where(x => condition.Statuses.Contains(x.Status));
                }
            }

            #endregion

            // Sort by properties.
            if (condition.Sort != null)
                categories =
                    _databaseFunction.Sort(categories, condition.Sort.Direction,
                        condition.Sort.Property);
            else
                categories = _databaseFunction.Sort(categories, SortDirection.Decending,
                    CategoriesSort.Name);

            // Result initialization.
            var result = new SearchResult<IList<Category>>();
            result.Total = await categories.CountAsync();
            result.Records = await _databaseFunction.Paginate(categories, condition.Pagination).ToListAsync();

            return Ok(result);
        }

        #endregion
    }
}