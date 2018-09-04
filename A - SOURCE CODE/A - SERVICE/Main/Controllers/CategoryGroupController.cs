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
using Main.ViewModels.CategoryGroup;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Services;
using Shared.Models;
using Shared.Resources;

namespace Main.Controllers
{
    [Route("api/[controller]")]
    public class CategoryGroupController : ApiBaseController
    {
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

        #region Constructures

        public CategoryGroupController(
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

        #region Methods

        /// <summary>
        ///     Add category group to system.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<IActionResult> AddCategoryGroup([FromBody] AddCategoryGroupViewModel info)
        {
            #region Parameters validation

            if (info == null)
            {
                info = new AddCategoryGroupViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Find category group

            // Find category group.
            var categoryGroups = UnitOfWork.CategoryGroups.Search();
            categoryGroups = categoryGroups.Where(x => x.Name == info.Name && x.Status == ItemStatus.Active);

            // Check whether category group exists or not.
            var bIsCategoryGroupAvailable = await categoryGroups.AnyAsync();
            if (!bIsCategoryGroupAvailable)
                return Conflict(new ApiResponse(HttpMessages.CategoryGroupCannotConflict));

            #endregion

            #region Category group initialization

            // Find identity from request.
            var identity = IdentityService.GetProfile(HttpContext);

            // Category group intialization.
            var categoryGroup = new CategoryGroup();
            categoryGroup.CreatorId = identity.Id;
            categoryGroup.Name = info.Name;
            categoryGroup.Description = info.Description;
            categoryGroup.Status = ItemStatus.Active;
            categoryGroup.CreatedTime = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);
            categoryGroup.LastModifiedTime = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Insert category group into system.
            UnitOfWork.CategoryGroups.Insert(categoryGroup);

            await UnitOfWork.CommitAsync();

            #endregion

            return Ok(categoryGroup);
        }

        /// <summary>
        /// Edit category group by using specific information.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCategoryGroup([FromRoute] int id, [FromBody] EditCategoryGroupViewModel info)
        {
            #region Parameters validation

            if (info == null)
            {
                info = new EditCategoryGroupViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Find category group

            // Get request identity.
            var identity = IdentityService.GetProfile(HttpContext);

            // Get all category group in database.
            var categoryGroups = UnitOfWork.CategoryGroups.Search();

            categoryGroups = categoryGroups.Where(x => x.Id == id && x.Status == ItemStatus.Active);

            // Get the first matched category group.
            var categoryGroup = await categoryGroups.FirstOrDefaultAsync();
            if (categoryGroup == null)
                return NotFound(new ApiResponse(HttpMessages.CategoryGroupNotFound));

            #endregion

            #region Update category group. information

            // Check whether information has been updated or not.
            var bHasInformationChanged = false;

            // Name is defined
            if (info.Name != null && info.Name != categoryGroup.Name)
            {
                categoryGroup.Name = info.Name;
                bHasInformationChanged = true;
            }

            // Description is defined
            if (info.Description != null && info.Description != categoryGroup.Description)
            {
                categoryGroup.Description = info.Description;
                bHasInformationChanged = true;
            }

            // Status is defined.
            if (info.Status != categoryGroup.Status)
            {
                categoryGroup.Status = info.Status;
                bHasInformationChanged = true;
            }

            if (bHasInformationChanged)
            {
                categoryGroup.LastModifiedTime = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);

                // Commit changes to database.
                await UnitOfWork.CommitAsync();
            }

            #endregion

            return Ok(categoryGroup);
        }

        /// <summary>
        ///     Load category group by using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("search")]
        public async Task<IActionResult> LoadCategoryGroups([FromBody] SearchCategoryGroupViewModel condition)
        {
            #region Parameters validation

            if (condition == null)
            {
                condition = new SearchCategoryGroupViewModel();
                TryValidateModel(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            // Find identity in request.
            var identity = IdentityService.GetProfile(HttpContext);

            #region Search for information

            // Get all category groups
            var categoryGroups = _unitOfWork.CategoryGroups.Search();

            // Id have been defined.
            if (condition.Ids != null && condition.Ids.Count > 0)
            {
                condition.Ids = condition.Ids.Where(x => x > 0).ToList();
                if (condition.Ids != null && condition.Ids.Count > 0)
                {
                    categoryGroups = categoryGroups.Where(x => condition.Ids.Contains(x.Id));
                }
            }

            // Creator Id have been defined.
            if (condition.CreatorIds != null && condition.CreatorIds.Count > 0)
            {
                condition.CreatorIds = condition.CreatorIds.Where(x => x > 0).ToList();
                if (condition.CreatorIds != null && condition.CreatorIds.Count > 0)
                {
                    categoryGroups = categoryGroups.Where(x => condition.CreatorIds.Contains(x.CreatorId));
                }
            }

            // Name have been defined.
            if (condition.Names != null && condition.Names.Count > 0)
            {
                condition.Names = condition.Names.Where(x => !string.IsNullOrEmpty(x)).ToList();
                if (condition.Names != null && condition.Names.Count > 0)
                {
                    categoryGroups = categoryGroups.Where(x => condition.Names.Any(y => x.Name.Contains(y)));
                }
            }

            // Description have been defined.
            if (condition.Descriptions != null && condition.Descriptions.Count > 0)
            {
                condition.Descriptions = condition.Descriptions.Where(x => !string.IsNullOrEmpty(x)).ToList();
                if (condition.Descriptions != null && condition.Descriptions.Count > 0)
                {
                    categoryGroups = categoryGroups.Where(x => condition.Descriptions.Any(y => x.Description.Contains(y)));
                }
            }

            // Search conditions which are based on roles.

            if (identity?.Role == AccountRole.Admin)
            {
                // Statuses have been defined.
                if (condition.Statuses != null && condition.Statuses.Count > 0)
                {
                    condition.Statuses =
                        condition.Statuses.Where(x => Enum.IsDefined(typeof(ItemStatus), x)).ToList();
                    if (condition.Statuses.Count > 0)
                        categoryGroups = categoryGroups.Where(x => condition.Statuses.Contains(x.Status));
                }
            }

            #endregion

            // Sort by properties.
            if (condition.Sort != null)
                categoryGroups =
                    _databaseFunction.Sort(categoryGroups, condition.Sort.Direction,
                        condition.Sort.Property);
            else
                categoryGroups = _databaseFunction.Sort(categoryGroups, SortDirection.Decending,
                    CategoryGroupSort.Name);

            // Result initialization.
            var result = new SearchResult<IList<CategoryGroup>>();
            result.Total = await categoryGroups.CountAsync();
            result.Records = await _databaseFunction.Paginate(categoryGroups, condition.Pagination).ToListAsync();

            return Ok(result);
        }

        #endregion
    }
}
