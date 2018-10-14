using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppBusiness.Interfaces;
using AppBusiness.Interfaces.Domains;
using AppDb.Interfaces;
using AppDb.Models.Entities;
using AutoMapper;
using Main.Constants.RealTime;
using Main.Interfaces.Services;
using Main.Interfaces.Services.RealTime;
using Main.Models.RealTime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Enumerations;
using Shared.Interfaces.Services;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.Category;

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
            IProfileService identityService,
            IRealTimeService realTimeService, ICategoryDomain categoryService) : base(unitOfWork, mapper, timeService,
            relationalDbService, identityService)
        {
            _realTimeService = realTimeService;
            _categoryService = categoryService;
        }

        #endregion

        #region Properties

        private readonly IRealTimeService _realTimeService;

        private readonly ICategoryDomain _categoryService;

        #endregion

        #region Methods

        /// <summary>
        ///     Add category to system.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<IActionResult> AddCategory([FromBody] AddCategoryViewModel model)
        {
            #region Parameters validation

            if (model == null)
            {
                model = new AddCategoryViewModel();
                TryValidateModel(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Find category

            // Find category.
            var categories = UnitOfWork.Categories.Search();
            categories = categories.Where(x => x.Name == model.Name && x.Status == ItemStatus.Active);

            // Check whether category exists or not.
            var bIsCategoryAvailable = await categories.AnyAsync();
            if (bIsCategoryAvailable)
                return Conflict(new ApiResponse(HttpMessages.CategoryCannotConflict));

            #endregion

            var category = await _categoryService.AddCategoryAsync(model);

            #region Real-time message broadcast

            // Send real-time message to all admins.
            var broadcastRealTimeMessageTask = _realTimeService.SendRealTimeMessageToGroupsAsync(
                new[] {RealTimeGroupConstant.Admin}, RealTimeEventConstant.AddCategory, category,
                CancellationToken.None);

            // Send push notification to all admin.
            var collapseKey = Guid.NewGuid().ToString("D");
            var realTimeMessage = new RealTimeMessage<Category>();
            realTimeMessage.Title = RealTimeMessages.AddNewCategoryTitle;
            realTimeMessage.Body = RealTimeMessages.AddNewCategoryContent;
            realTimeMessage.AdditionalInfo = category;

            var broadcastPushMessageTask = _realTimeService.SendPushMessageToGroupsAsync(
                new[] {RealTimeGroupConstant.Admin}, collapseKey, realTimeMessage);

            await Task.WhenAll(broadcastRealTimeMessageTask, broadcastPushMessageTask);

            #endregion

            return Ok(category);
        }

        /// <summary>
        ///     Edit category by using specific information.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCategory([FromRoute] int id, [FromBody] EditCategoryViewModel model)
        {
            #region Parameters validation

            if (model == null)
            {
                model = new EditCategoryViewModel();
                TryValidateModel(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            // Edit category asynchronously.
            var category = await _categoryService.EditCategoryAsync(id, model);

            #region Real-time message broadcast

            // Send real-time message to all admins.
            var broadcastRealTimeMessageTask = _realTimeService.SendRealTimeMessageToGroupsAsync(
                new[] {RealTimeGroupConstant.Admin}, RealTimeEventConstant.EditCategory, category,
                CancellationToken.None);

            // Send push notification to all admin.
            var collapseKey = Guid.NewGuid().ToString("D");
            var realTimeMessage = new RealTimeMessage<Category>();
            realTimeMessage.Title = RealTimeMessages.EditCategoryTitle;
            realTimeMessage.Body = RealTimeMessages.EditCategoryContent;
            realTimeMessage.AdditionalInfo = category;

            var broadcastPushMessageTask = _realTimeService.SendPushMessageToGroupsAsync(
                new[] {RealTimeGroupConstant.Admin}, collapseKey, realTimeMessage);

            await Task.WhenAll(broadcastRealTimeMessageTask, broadcastPushMessageTask);

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

            var loadCategoriesResult = await _categoryService.SearchCategoriesAsync(condition, CancellationToken.None);
            return Ok(loadCategoriesResult);
        }

        /// <summary>
        ///     Load category using specific id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> LoadCategoryUsingId([FromRoute] int id)
        {
            var loadCategoryCondition = new SearchCategoryViewModel();
            loadCategoryCondition.Ids = new HashSet<int> {id};
            loadCategoryCondition.Pagination = new Pagination(1, 1);

            var category = await _categoryService.GetCategoryUsingIdAsync(id, CancellationToken.None);
            if (category == null)
                return NotFound(new ApiResponse(HttpMessages.CategoryNotFound));

            return Ok(category);
        }

        #endregion
    }
}