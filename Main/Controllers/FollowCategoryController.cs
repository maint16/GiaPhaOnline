using System.Threading.Tasks;
using AppBusiness.Interfaces;
using AppBusiness.Interfaces.Domains;
using AppDb.Interfaces;
using AppShared.ViewModels.FollowCategory;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ServiceShared.Interfaces.Services;

namespace Main.Controllers
{
    [Route("api/follow-category")]
    public class FollowCategoryController : Controller
    {
        #region Properties

        private readonly IFollowCategoryDomain _followCategoryDomain;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initialize controller with injectors.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="mapper"></param>
        /// <param name="identityService"></param>
        /// <param name="timeService"></param>
        /// <param name="databaseFunction"></param>
        /// <param name="followCategoryDomain"></param>
        public FollowCategoryController(IAppUnitOfWork unitOfWork, IMapper mapper, IAppProfileService identityService,
            ITimeService timeService, IBaseRelationalDbService databaseFunction,
            IFollowCategoryDomain followCategoryDomain)
        {
            _followCategoryDomain = followCategoryDomain;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Start following a category.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<IActionResult> FollowCategory([FromQuery] AddFollowCategoryViewModel model)
        {
            var followCategory = await _followCategoryDomain.AddFollowCategoryAsync(model);
            return Ok(followCategory);
        }

        /// <summary>
        ///     Stop following a category.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [HttpDelete("")]
        public async Task<IActionResult> StopFollowingCategory([FromRoute] int categoryId)
        {
            var deleteFollowingCategoryModel = new DeleteFollowCategoryViewModel();
            deleteFollowingCategoryModel.CategoryId = categoryId;

            await _followCategoryDomain.DeleteFollowCategoryAsync(deleteFollowingCategoryModel);
            return Ok();
        }

        /// <summary>
        ///     Search for following category by using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("search")]
        public async Task<IActionResult> SearchForFollowingCategories(
            [FromBody] SearchFollowCategoryViewModel condition)
        {
            if (condition == null)
            {
                condition = new SearchFollowCategoryViewModel();
                TryValidateModel(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var loadFollowCategoriesResult = await _followCategoryDomain.SearchFollowCategoriesAsync(condition);
            return Ok(loadFollowCategoriesResult);
        }

        #endregion
    }
}