using System;
using System.Threading;
using System.Threading.Tasks;
using AppBusiness.Interfaces;
using AppBusiness.Interfaces.Domains;
using AppBusiness.Models.NotificationMessages;
using AppDb.Interfaces;
using AppDb.Models.Entities;
using AppModel.Enumerations;
using AppShared.Resources;
using AppShared.ViewModels.CategoryGroup;
using AutoMapper;
using Main.Constants;
using Main.Constants.RealTime;
using Main.Interfaces.Services.RealTime;
using Main.Models.AdditionalMessageInfo;
using Main.Models.RealTime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceShared.Authentications.ActionFilters;
using ServiceShared.Exceptions;
using ServiceShared.Interfaces.Services;

namespace Main.Controllers
{
    [Route("api/category-group")]
    public class CategoryGroupController : ApiBaseController
    {
        #region Constructures

        public CategoryGroupController(
            IAppUnitOfWork unitOfWork,
            IMapper mapper,
            IBaseTimeService baseTimeService,
            IBaseRelationalDbService relationalDbService,
            IBaseEncryptionService encryptionService,
            IAppProfileService profileService, IRealTimeService realTimeService,
            ICategoryGroupDomain categoryGroupService,
            INotificationMessageDomain notificationMessageDomain, IAppProfileService appProfileService) : base(unitOfWork, mapper, baseTimeService,
            relationalDbService, profileService)
        {
            _realTimeService = realTimeService;
            _categoryGroupService = categoryGroupService;
            _mapper = mapper;
            _notificationMessageDomain = notificationMessageDomain;
            _appProfileService = appProfileService;
        }

        #endregion

        #region Properties

        private readonly IRealTimeService _realTimeService;

        private readonly ICategoryGroupDomain _categoryGroupService;

        private readonly IMapper _mapper;

        /// <summary>
        /// Notification message
        /// </summary>
        private readonly INotificationMessageDomain _notificationMessageDomain;

        private readonly IAppProfileService _appProfileService;
        #endregion

        #region Methods

        /// <summary>
        ///     Add category group to system.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("")]
        [Authorize(Policy = PolicyConstant.IsAdminPolicy)]
        public async Task<IActionResult> AddCategoryGroup([FromBody] AddCategoryGroupViewModel model)
        {
            #region Parameters validation

            if (model == null)
            {
                model = new AddCategoryGroupViewModel();
                TryValidateModel(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            // Get requester profile.

            var categoryGroup = await _categoryGroupService.AddCategoryGroup(model);
            var profile = _appProfileService.GetProfile();

            #region Real-time message broadcast

            // Send real-time message to all admins.
            var broadcastRealTimeMessageTask = _realTimeService.SendRealTimeMessageToGroupsAsync(
                new[] {RealTimeGroupConstant.Admin}, RealTimeEventConstant.AddCategoryGroup, categoryGroup,
                CancellationToken.None);

            // Send push notification to all admin.
            var collapseKey = Guid.NewGuid().ToString("D");
            var realTimeMessage = new RealTimeMessage<CategoryGroup>();
            realTimeMessage.Title = RealTimeMessages.AddNewCategoryGroupTitle;
            realTimeMessage.Body = RealTimeMessages.AddNewCategoryGroupContent;
            realTimeMessage.AdditionalInfo = categoryGroup;

            var broadcastPushMessageTask = _realTimeService.SendPushMessageToGroupsAsync(
                new[] {RealTimeGroupConstant.Admin}, collapseKey, realTimeMessage);

            await Task.WhenAll(broadcastRealTimeMessageTask, broadcastPushMessageTask);

            #endregion

            #region Notification
            
            var additionalInfo = new AddCategoryGroupAdditionalInfoModel();
            additionalInfo.CategoryGroupName = model.Name;
            additionalInfo.CreatorName = profile.Nickname;
            await _notificationMessageDomain.AddNotificationMessageToUserGroup(UserGroup.Admin,
                new AddUserGroupNotificationMessageModel<AddCategoryGroupAdditionalInfoModel>(additionalInfo,
                    NotificationMessages.SomeoneCreatedCategoryGroup));

            #endregion

            return Ok(categoryGroup);
        }

        /// <summary>
        ///     Edit category group by using specific information.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Policy = PolicyConstant.IsAdminPolicy)]
        public async Task<IActionResult> EditCategoryGroup([FromRoute] int id,
            [FromBody] EditCategoryGroupViewModel model)
        {
            #region Parameters validation

            if (model == null)
            {
                model = new EditCategoryGroupViewModel();
                TryValidateModel(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Update category group. information

            try
            {
                var categoryGroup = await _categoryGroupService.EditCategoryGroup(id, model);

                // Send real-time message to all admins.
                var broadcastRealTimeMessageTask = _realTimeService.SendRealTimeMessageToGroupsAsync(
                    new[] {RealTimeGroupConstant.Admin}, RealTimeEventConstant.EditCategoryGroup, categoryGroup,
                    CancellationToken.None);

                // Send push notification to all admin.
                var collapseKey = Guid.NewGuid().ToString("D");
                var realTimeMessage = new RealTimeMessage<CategoryGroup>();
                realTimeMessage.Title = RealTimeMessages.EditCategoryGroupTitle;
                realTimeMessage.Body = RealTimeMessages.EditCategoryGroupContent;
                realTimeMessage.AdditionalInfo = categoryGroup;

                var broadcastPushMessageTask = _realTimeService.SendPushMessageToGroupsAsync(
                    new[] {RealTimeGroupConstant.Admin}, collapseKey, realTimeMessage);

                await Task.WhenAll(broadcastRealTimeMessageTask, broadcastPushMessageTask);
                return Ok(categoryGroup);
            }
            catch (Exception exception)
            {
                if (!(exception is NotModifiedException))
                    return Ok();

                throw;
            }

            #endregion
        }

        /// <summary>
        ///     Load category group by using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("search")]
        [ByPassAuthorization]
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

            var loadCategoryGroupsResult = await _categoryGroupService.SearchCategoryGroupsAsync(condition);
            return Ok(loadCategoryGroupsResult);
        }

        #endregion
    }
}