using System.Threading.Tasks;
using AutoMapper;
using MainBusiness.Interfaces;
using MainBusiness.Interfaces.Domains;
using MainBusiness.Models.NotificationMessages;
using MainDb.Interfaces;
using MainMicroService.Models.AdditionalMessageInfo.Category;
using MainModel.Enumerations;
using MainShared.Resources;
using MainShared.ViewModels.FollowCategory;
using Microsoft.AspNetCore.Mvc;
using ServiceShared.Interfaces.Services;

namespace MainMicroService.Controllers
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
        /// <param name="baseTimeService"></param>
        /// <param name="databaseFunction"></param>
        /// <param name="followCategoryDomain"></param>
        /// <param name="categoryDomain"></param>
        /// <param name="appProfileService"></param>
        /// <param name="notificationMessageDomain"></param>
        public FollowCategoryController(IAppUnitOfWork unitOfWork, IMapper mapper, IAppProfileService identityService,
            IBaseTimeService baseTimeService, IBaseRelationalDbService databaseFunction,
            IFollowCategoryDomain followCategoryDomain,
            ICategoryDomain categoryDomain,
            IAppProfileService appProfileService,
            INotificationMessageDomain notificationMessageDomain)
        {
            _followCategoryDomain = followCategoryDomain;
            _appProfileService = appProfileService;
            _notificationMessageDomain = notificationMessageDomain;
            _categoryDomain = categoryDomain;
        }

        #endregion

        #region Properties

        private readonly IFollowCategoryDomain _followCategoryDomain;

        private readonly ICategoryDomain _categoryDomain;

        private readonly IAppProfileService _appProfileService;

        /// <summary>
        ///     Notification message
        /// </summary>
        private readonly INotificationMessageDomain _notificationMessageDomain;

        #endregion

        #region Methods

        /// <summary>
        ///     Start following a category.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("{categoryId}")]
        public async Task<IActionResult> FollowCategory([FromRoute] AddFollowCategoryViewModel model)
        {
            var followCategory = await _followCategoryDomain.AddFollowCategoryAsync(model);

            var category = await _categoryDomain.GetCategoryUsingIdAsync(model.CategoryId);

            // Get requester profile.
            var profile = _appProfileService.GetProfile();

            #region Notification

            var additionalInfo = new FollowCategoryAdditionalInfoModel();
            additionalInfo.CategoryName = category.Name;
            additionalInfo.FollowerName = profile.Nickname;
            await _notificationMessageDomain.AddNotificationMessageToUserGroup(UserGroup.Admin,
                new AddUserGroupNotificationMessageModel<FollowCategoryAdditionalInfoModel>(additionalInfo,
                    NotificationMessages.SomeoneFollowedCategory));

            #endregion

            return Ok(followCategory);
        }

        /// <summary>
        ///     Stop following a category.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [HttpDelete("{categoryId}")]
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