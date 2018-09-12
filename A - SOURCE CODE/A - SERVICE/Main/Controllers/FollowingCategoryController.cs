using System;
using System.Linq;
using System.Threading.Tasks;
using AppDb.Interfaces;
using AppDb.Interfaces.Repositories;
using AppDb.Models.Entities;
using AppModel.Enumerations;
using AutoMapper;
using Main.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Services;
using Shared.Models;
using Shared.Resources;

namespace Main.Controllers
{
    [Route("api/following-category")]
    public class FollowingCategoryController : Controller
    {
        #region Constructors

        /// <summary>
        ///     Initialize controller with injectors.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="mapper"></param>
        /// <param name="identityService"></param>
        /// <param name="timeService"></param>
        /// <param name="databaseFunction"></param>
        public FollowingCategoryController(IUnitOfWork unitOfWork, IMapper mapper, IIdentityService identityService,
            ITimeService timeService, IRelationalDbService databaseFunction)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _identityService = identityService;
            _timeService = timeService;
            _databaseFunction = databaseFunction;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Instance which is for accessing to database.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        ///     Instance which is for accessing automapper.
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        ///     Instance which is for accessing identity attached in request.
        /// </summary>
        private readonly IIdentityService _identityService;

        /// <summary>
        ///     Service which is for time calculation.
        /// </summary>
        private readonly ITimeService _timeService;

        /// <summary>
        ///     Instance to access generic database function.
        /// </summary>
        private readonly IRelationalDbService _databaseFunction;

        #endregion

        //#region Methods

        /// <summary>
        /// Start following a category.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<IActionResult> FollowCategory([FromQuery] int categoryId)
        {
            #region Find category

            // Find categories.
            var categories = _unitOfWork.Categories.Search();
            categories = categories.Where(x => x.Id == categoryId && x.Status == ItemStatus.Active);

            // Find the first matched result.
            var category = await categories.FirstOrDefaultAsync();
            if (category == null)
                return NotFound(new ApiResponse(HttpMessages.CategoryNotFound));

            #endregion

            #region Check whether user already followed category or not

            // Find request identity.
            var identity = _identityService.GetProfile(HttpContext);

            // Find follow categories.
            var followCategories = _unitOfWork.FollowCategories.Search();
            followCategories = followCategories.Where(x => x.CategoryId == categoryId && x.FollowerId == identity.Id);
            var followCategory = await followCategories.FirstOrDefaultAsync();

            #endregion

            #region Follow category initalization

            // Already followed the category.
            if (followCategory != null)
                followCategory.Status = FollowStatus.Ignore;
            else
            {
                // Initialize follow category.
                followCategory = new FollowCategory();
                followCategory.FollowerId = identity.Id;
                followCategory.CategoryId = categoryId;
                followCategory.Status = FollowStatus.Following;
                followCategory.CreatedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

                // Insert to system.
                _unitOfWork.FollowCategories.Insert(followCategory);
            }

            // Commit changes.
            await _unitOfWork.CommitAsync();

            #endregion

            return Ok(followCategory);
        }

        ///// <summary>
        ///// Delete a specific post.
        ///// </summary>
        ///// <param name="categoryId"></param>
        ///// <returns></returns>
        //[HttpDelete("")]
        //public async Task<IActionResult> StopFollowingCategory([FromRoute] int categoryId)
        //{
        //    // Find request identity.
        //    var identity = _identityService.GetProfile(HttpContext);

        //    // Find categories by using specific conditions.
        //    var followCategories = _unitOfWork.FollowCategories.Search();
        //    followCategories = followCategories.Where(x => x.CategoryId == categoryId && x.FollowerId == identity.Id);

        //    // Find the first matched category.
        //    var followCategory = await followCategories.FirstOrDefaultAsync();
        //    if (followCategory == null)
        //        return NotFound(new ApiResponse(HttpMessages.CategoryNotFound));

        //    // Stop following category.
        //    followCategory.Status = ItemStatus.NotAvailable;

        //    // Save changes.
        //    await _unitOfWork.CommitAsync();

        //    return Ok();
        //}

        ///// <summary>
        ///// Search for posts by using specific conditions.
        ///// </summary>
        ///// <param name="condition"></param>
        ///// <returns></returns>
        //[HttpPost("search")]
        //public async Task<IActionResult> SearchForPosts([FromBody] SearchFollowCategoryViewModel condition)
        //{
        //    #region Parameters validation

        //    if (condition == null)
        //    {
        //        condition = new SearchFollowCategoryViewModel();
        //        TryValidateModel(condition);
        //    }

        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    #endregion

        //    #region Search for information

        //    // Find identity in request.
        //    var identity = _identityService.GetProfile(HttpContext);

        //    // Search for posts.
        //    var followCategories = _unitOfWork.FollowCategories.Search();

        //    // Category id is defined.
        //    if (condition.CategoryIds != null && condition.CategoryIds.Count > 0)
        //    {
        //        var categoryIds = condition.CategoryIds.Where(x => x > 0).ToList();
        //        if (categoryIds.Count > 0)
        //            followCategories = followCategories.Where(x => condition.CategoryIds.Contains(x.CategoryId));
        //    }
        //    //if (condition.CategoryIds != null)
        //    //    followCategories = followCategories.Where(x => x.CategoryId == condition.CategoryIds.Value);

        //    // Search conditions which are based on roles.
        //    if (identity.Role == AccountRole.Admin)
        //    {
        //        // Follower id is defined.
        //        if (condition.FollowerId != null)
        //            followCategories = followCategories.Where(x => x.FollowerId == condition.FollowerId.Value);


        //    }
        //    else
        //    {
        //        // Normal users can his/her followed categories.
        //        followCategories = followCategories.Where(x => x.FollowerId == identity.Id);
        //    }

        //    // Statuses are defined.
        //    if (condition.Statuses != null && condition.Statuses.Count > 0)
        //        followCategories = followCategories.Where(x => condition.Statuses.Contains(x.Status));

        //    // Created time has been defined.
        //    var createdTime = condition.CreatedTime;
        //    if (createdTime != null)
        //    {
        //        var from = createdTime.From;
        //        var to = createdTime.To;

        //        if (from != null)
        //            followCategories = _databaseFunction.SearchNumericProperty(followCategories, x => x.CreatedTime, from.Value,
        //                NumericComparision.GreaterEqual);

        //        if (to != null)
        //            followCategories = _databaseFunction.SearchNumericProperty(followCategories, x => x.CreatedTime, to.Value,
        //                NumericComparision.LowerEqual);
        //    }

        //    // Sort property & direction.
        //    var sort = condition.Sort;
        //    if (sort != null)
        //        followCategories = _databaseFunction.Sort(followCategories, sort.Direction, sort.Property);
        //    else
        //        followCategories = _databaseFunction.Sort(followCategories, SortDirection.Decending, FollowCategorySort.CreatedTime);

        //    var result = new SearchResult<IList<FollowCategory>>();
        //    result.Total = await followCategories.CountAsync();
        //    result.Records = await _databaseFunction.Paginate(followCategories, condition.Pagination).ToListAsync();

        //    #endregion

        //    return Ok(result);
        //}

        //#endregion
    }
}