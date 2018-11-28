using System;
using System.Threading.Tasks;
using AutoMapper;
using MainBusiness.Interfaces;
using MainBusiness.Interfaces.Domains;
using MainBusiness.Models.NotificationMessages;
using MainDb.Interfaces;
using MainMicroService.Models.AdditionalMessageInfo.Topic;
using MainModel.Enumerations;
using MainShared.Resources;
using MainShared.ViewModels.ReportTopic;
using Microsoft.AspNetCore.Mvc;
using ServiceShared.Exceptions;
using ServiceShared.Interfaces.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MainMicroService.Controllers
{
    [Route("api/report-topic")]
    public class ReportTopicController : ApiBaseController
    {
        #region Constructures

        public ReportTopicController(
            IAppUnitOfWork unitOfWork,
            IMapper mapper,
            IBaseTimeService baseTimeService,
            IBaseRelationalDbService relationalDbService,
            IBaseEncryptionService encryptionService,
            IAppProfileService profileService, ITopicReportDomain topicReportDomain,
            ITopicDomain topicDomain,
            INotificationMessageDomain notificationMessageDomain) : base(unitOfWork, mapper,
            baseTimeService,
            relationalDbService, profileService)
        {
            _topicReportDomain = topicReportDomain;
            _appProfileService = profileService;
            _topicDomain = topicDomain;
            _notificationMessageDomain = notificationMessageDomain;
        }

        #endregion

        #region Properties

        private readonly ITopicReportDomain _topicReportDomain;

        private readonly ITopicDomain _topicDomain;

        private readonly IAppProfileService _appProfileService;

        /// <summary>
        ///     Notification message
        /// </summary>
        private readonly INotificationMessageDomain _notificationMessageDomain;

        #endregion

        #region Methods

        /// <summary>
        ///     Report a topic.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<IActionResult> AddReportTopic([FromBody] AddReportTopicViewModel model)
        {
            if (model == null)
            {
                model = new AddReportTopicViewModel();
                TryValidateModel(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var topicReport = await _topicReportDomain.AddTopicReportAsync(model);

            // Get requester profile.
            var profile = _appProfileService.GetProfile();

            var topic = _topicDomain.GetTopicUsingIdAsync(model.TopicId);

            #region Notification

            var additionalInfo = new ReportTopicAdditionalInfoModel();
            additionalInfo.TopicName = topic.Result.Title;
            additionalInfo.ReporterName = profile.Nickname;
            await _notificationMessageDomain.AddNotificationMessageToUserGroup(UserGroup.Admin,
                new AddUserGroupNotificationMessageModel<ReportTopicAdditionalInfoModel>(additionalInfo,
                    NotificationMessages.SomeoneReportedTopic));

            #endregion

            return Ok(topicReport);
        }

        /// <summary>
        ///     Edit reply by using specific information.
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{topicId}")]
        public async Task<IActionResult> EditTopicReport([FromRoute] int topicId,
            [FromBody] EditReportTopicViewModel model)
        {
            if (model == null)
            {
                model = new EditReportTopicViewModel();
                TryValidateModel(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var topicReport = await _topicReportDomain.EditTopicReportAsync(topicId, model);
                return Ok(topicReport);
            }
            catch (Exception exception)
            {
                if (!(exception is NotModifiedException))
                    throw;

                return Ok();
            }
        }

        /// <summary>
        ///     Search topic report by using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("search")]
        public async Task<IActionResult> LoadReportTopics([FromBody] SearchReportTopicViewModel condition)
        {
            #region Parameters validation

            if (condition == null)
            {
                condition = new SearchReportTopicViewModel();
                TryValidateModel(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            var loadTopicReportsResult = await _topicReportDomain.SearchTopicReportsAsync(condition);
            return Ok(loadTopicReportsResult);
        }

        #endregion
    }
}