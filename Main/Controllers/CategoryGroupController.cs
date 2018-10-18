using System;
using System.Threading;
using System.Threading.Tasks;
using AppBusiness.Interfaces;
using AppBusiness.Interfaces.Domains;
using AppDb.Interfaces;
using AppDb.Models.Entities;
using AppShared.Resources;
using AppShared.ViewModels.CategoryGroup;
using AutoMapper;
using Main.Authentications.ActionFilters;
using Main.Constants;
using Main.Constants.RealTime;
using Main.Interfaces.Services.RealTime;
using Main.Models.RealTime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            ITimeService timeService,
            IBaseRelationalDbService relationalDbService,
            IEncryptionService encryptionService,
            IProfileService identityService, IRealTimeService realTimeService,
            ICategoryGroupDomain categoryGroupService) : base(unitOfWork, mapper, timeService,
            relationalDbService, identityService)
        {
            _realTimeService = realTimeService;
            _categoryGroupService = categoryGroupService;
        }

        #endregion

        #region Properties

        private readonly IRealTimeService _realTimeService;

        private readonly ICategoryGroupDomain _categoryGroupService;

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

            var categoryGroup = await _categoryGroupService.AddCategoryGroup(model);

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