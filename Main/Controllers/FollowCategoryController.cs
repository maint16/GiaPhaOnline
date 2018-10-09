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
using Main.Interfaces.Services.Businesses;
using Main.ViewModels.FollowCategory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Services;
using Shared.Models;
using Shared.Resources;

namespace Main.Controllers
{
    [Route("api/follow-category")]
    public class FollowCategoryController : Controller
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
        /// <param name="followCategoryService"></param>
        public FollowCategoryController(IUnitOfWork unitOfWork, IMapper mapper, IIdentityService identityService,
            ITimeService timeService, IRelationalDbService databaseFunction, IFollowCategoryService followCategoryService)
        {
            _followCategoryService = followCategoryService;
        }

        #endregion

        #region Properties
        
        private readonly IFollowCategoryService _followCategoryService;

        #endregion

        #region Methods

        /// <summary>
        /// Start following a category.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<IActionResult> FollowCategory([FromQuery] int categoryId)
        {
            var addFollowCategoryModel = new AddFollowCategoryViewModel();
            addFollowCategoryModel.CategoryId = categoryId;
            var followCategory = await _followCategoryService.AddFollowCategoryAsync(addFollowCategoryModel);
            return Ok(followCategory);
        }

        /// <summary>
        /// Stop following a category.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [HttpDelete("")]
        public async Task<IActionResult> StopFollowingCategory([FromRoute] int categoryId)
        {
            var deleteFollowingCategoryModel = new DeleteFollowCategoryViewModel();
            deleteFollowingCategoryModel.CategoryId = categoryId;

            await _followCategoryService.DeleteFollowCategoryAsync(deleteFollowingCategoryModel);
            return Ok();
        }

        /// <summary>
        /// Search for following category by using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("search")]
        public async Task<IActionResult> SearchForFollowingCategories([FromBody] SearchFollowCategoryViewModel condition)
        {
            if (condition == null)
            {
                condition = new SearchFollowCategoryViewModel();
                TryValidateModel(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var loadFollowCategoriesResult = await _followCategoryService.SearchFollowCategoriesAsync(condition);
            return Ok(loadFollowCategoriesResult);
        }

        #endregion
    }
}
