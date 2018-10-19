using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppBusiness.Interfaces;
using AppBusiness.Interfaces.Domains;
using AppDb.Interfaces;
using AppDb.Models.Entities;
using AppShared.Resources;
using AppShared.ViewModels.Category;
using AutoMapper;
using ClientShared.Enumerations;
using ClientShared.Models;
using Main.Constants;
using Main.Constants.RealTime;
using Main.Interfaces.Services.RealTime;
using Main.Models.RealTime;
using Main.ViewModels.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServiceShared.Interfaces.Services;
using ServiceShared.Models;
using SkiaSharp;

namespace Main.Controllers
{
    [Route("api/[controller]")]
    public class CategoryController : ApiBaseController
    {
        #region Constructors

        public CategoryController(
            IAppUnitOfWork unitOfWork,
            IMapper mapper,
            ITimeService timeService,
            IBaseRelationalDbService relationalDbService,
            IEncryptionService encryptionService,
            IAppProfileService profileService,
            IRealTimeService realTimeService, ICategoryDomain categoryDomain, ILogger<CategoryController> logger) :
            base(unitOfWork, mapper, timeService,
                relationalDbService, profileService)
        {
            _realTimeService = realTimeService;
            _categoryDomain = categoryDomain;
            _logger = logger;
        }

        #endregion

        #region Properties

        private readonly IRealTimeService _realTimeService;

        private readonly ICategoryDomain _categoryDomain;

        private readonly ILogger _logger;

        #endregion

        #region Methods

        /// <summary>
        ///     Add category to system.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("")]
        [Authorize(Policy = PolicyConstant.IsAdminPolicy)]
        public virtual async Task<IActionResult> AddCategory([FromBody] AddCategoryViewModel model)
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

            var category = await _categoryDomain.AddCategoryAsync(model);

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
        [Authorize(Policy = PolicyConstant.IsAdminPolicy)]
        public virtual async Task<IActionResult> EditCategory([FromRoute] int id,
            [FromBody] EditCategoryViewModel model)
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
            var category = await _categoryDomain.EditCategoryAsync(id, model);

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
        ///     Delete a category.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("")]
        [Authorize(Policy = PolicyConstant.IsAdminPolicy)]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id)
        {
            var deleteCategoryViewModel = new DeleteCategoryViewModel
            {
                Id = id
            };

            await _categoryDomain.DeleteCategoryAsync(deleteCategoryViewModel);

            return Ok();
        }

        /// <summary>
        ///     Load category by using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("search")]
        public virtual async Task<IActionResult> LoadCategories([FromBody] SearchCategoryViewModel condition)
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

            var loadCategoriesResult = await _categoryDomain.SearchCategoriesAsync(condition, CancellationToken.None);
            return Ok(loadCategoriesResult);
        }

        /// <summary>
        ///     Load category using specific id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public virtual async Task<IActionResult> LoadCategoryUsingId([FromRoute] int id)
        {
            var loadCategoryCondition = new SearchCategoryViewModel();
            loadCategoryCondition.Ids = new HashSet<int> {id};
            loadCategoryCondition.Pagination = new Pagination(1, 1);

            var category = await _categoryDomain.GetCategoryUsingIdAsync(id, CancellationToken.None);
            if (category == null)
                return NotFound(new ApiResponse(HttpMessages.CategoryNotFound));

            return Ok(category);
        }

        /// <summary>
        ///     Upload category using specific information.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("photo")]
        [Authorize(Policy = PolicyConstant.IsAdminPolicy)]
        public virtual async Task<IActionResult> UploadCategoryPhoto(UploadCategoryPhotoViewModel model)
        {
            if (model == null)
            {
                model = new UploadCategoryPhotoViewModel();
                TryValidateModel(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Reflect image variable.
            var image = model.Photo;

            using (var skManagedStream = new SKManagedStream(image.OpenReadStream()))
            {
                var skBitmap = SKBitmap.Decode(skManagedStream);

                try
                {
                    var user = await _categoryDomain.UploadCategoryPhotoAsync(model.CategoryId, skBitmap);
                    return Ok(user);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception.Message, exception);
                    return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse(HttpMessages.ImageIsInvalid));
                }
            }
        }

        #endregion
    }
}