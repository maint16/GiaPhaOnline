using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppBusiness.Interfaces;
using AppBusiness.Interfaces.Domains;
using AppBusiness.Models.NotificationMessages;
using AppDb.Interfaces;
using AppDb.Models.Entities;
using AppShared.Resources;
using AppShared.ViewModels.Topic;
using AutoMapper;
using ClientShared.Enumerations;
using Main.Constants;
using Main.Constants.RealTime;
using Main.Interfaces.Services;
using Main.Interfaces.Services.RealTime;
using Main.Models.RealTime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServiceShared.Interfaces.Services;
using ServiceShared.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Main.Controllers
{
    [Route("api/[controller]")]
    public class TopicController : Controller
    {
        #region Constructors

        public TopicController(
            ITimeService timeService,
            IBaseRelationalDbService relationalDbService,
            IEncryptionService encryptionService,
            IProfileService identityService,
            ISendMailService sendMailService,
            IEmailCacheService emailCacheService,
            IRealTimeService realTimeService,
            ILogger<TopicController> logger,
            ITopicDomain topicDomain, INotificationMessageDomain notificationMessageDomain, IMapper mapper,
            IAppUnitOfWork unitOfWork)
        {
            _sendMailService = sendMailService;
            _emailCacheService = emailCacheService;
            _logger = logger;
            _topicDomain = topicDomain;
            _realTimeService = realTimeService;
            _notificationMessageDomain = notificationMessageDomain;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Send email service
        /// </summary>
        private readonly ISendMailService _sendMailService;

        /// <summary>
        ///     Email cache service.
        /// </summary>
        private readonly IEmailCacheService _emailCacheService;

        /// <summary>
        ///     Logging instance.
        /// </summary>
        private readonly ILogger _logger;

        private readonly IMapper _mapper;

        private readonly ITopicDomain _topicDomain;

        private readonly IAppUnitOfWork _unitOfWork;

        /// <summary>
        ///     Real time service
        /// </summary>
        private readonly IRealTimeService _realTimeService;

        private readonly INotificationMessageDomain _notificationMessageDomain;

        #endregion

        #region Methods

        /// <summary>
        ///     Add topic to system.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<IActionResult> AddTopic([FromBody] AddTopicViewModel model)
        {
            if (model == null)
            {
                model = new AddTopicViewModel();
                TryValidateModel(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Add topic.
            var topic = await _topicDomain.AddTopicAsync(model, CancellationToken.None);

            // Add notification
            var clonedTopic = _mapper.Map<Topic>(topic);
            clonedTopic.Title = null;
            clonedTopic.Body = null;

            await _notificationMessageDomain.AddNotificationMessageAsync(
                new AddNotificationMessageModel<Topic>(clonedTopic.OwnerId, clonedTopic,
                    NotificationMessages.SomeoneCommentedTopic));
            return Ok(topic);
        }

        /// <summary>
        ///     Edit topic by using specific information.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditTopic([FromRoute] int id, [FromBody] EditTopicViewModel model)
        {
            #region Parameters validation

            if (model == null)
            {
                model = new EditTopicViewModel();
                TryValidateModel(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            // Update topic information.
            var topic = await _topicDomain.EditTopicAsync(id, model);

            if (topic.Status != ItemStatus.Disabled)
                return Ok(topic);

            var users = _unitOfWork.Accounts.Search();
            users = users.Where(x => x.Id == topic.OwnerId);
            var user = await users.FirstOrDefaultAsync();

            if (user != null)
            {
                var emailTemplate = _emailCacheService.Read(EmailTemplateConstant.DeleteTopic);
                if (emailTemplate != null)
                {
                    await _sendMailService.SendAsync(new HashSet<string> {user.Email}, null, null,
                        emailTemplate.Subject,
                        emailTemplate.Content, true, CancellationToken.None);

                    _logger.LogInformation($"Sent message to {user.Email} with subject {emailTemplate.Subject}");
                }
            }

            return Ok(topic);
        }

        /// <summary>
        ///     Delete a topic.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("")]
        [Authorize(Policy = PolicyConstant.IsAdminPolicy)]
        public async Task<IActionResult> DeleteTopic([FromRoute] int id)
        {
            var deleteTopicViewModel = new DeleteTopicViewModel
            {
                Id = id
            };

            await _topicDomain.DeleteTopicAsync(deleteTopicViewModel);

            var topic = await _topicDomain.GetTopicUsingIdAsync(id, CancellationToken.None);

            #region Send email 

            if (topic == null)
                return NotFound(new ApiResponse(HttpMessages.TopicNotFound));

            var users = _unitOfWork.Accounts.Search();

            users = users.Where(x => x.Id == topic.OwnerId);

            var user = await users.FirstOrDefaultAsync();

            if (user != null)
            {
                var emailTemplate = _emailCacheService.Read(EmailTemplateConstant.DeleteTopic);
                if (emailTemplate != null)
                {
                    await _sendMailService.SendAsync(new HashSet<string> {user.Email}, null, null,
                        emailTemplate.Subject,
                        emailTemplate.Content, true, CancellationToken.None);

                    _logger.LogInformation($"Sent message to {user.Email} with subject {emailTemplate.Subject}");
                }
            }

            #endregion

            #region Real-time message broadcast

            // Send real-time message to all admins.
            var broadcastRealTimeMessageTask = _realTimeService.SendRealTimeMessageToGroupsAsync(
                new[] {RealTimeGroupConstant.Admin}, RealTimeEventConstant.DeleteTopic, topic,
                CancellationToken.None);

            // Send push notification to all admin.
            var collapseKey = Guid.NewGuid().ToString("D");
            var realTimeMessage = new RealTimeMessage<Topic>();
            realTimeMessage.Title = RealTimeMessages.DeleteTopicTitle;
            realTimeMessage.Body = RealTimeMessages.DeleteTopicContent;
            realTimeMessage.AdditionalInfo = topic;

            var broadcastPushMessageTask = _realTimeService.SendPushMessageToGroupsAsync(
                new[] {RealTimeGroupConstant.Admin}, collapseKey, realTimeMessage);

            await Task.WhenAll(broadcastRealTimeMessageTask, broadcastPushMessageTask);

            #endregion


            return Ok();
        }

        /// <summary>
        ///     Load topic by using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("search")]
        public async Task<IActionResult> LoadTopics([FromBody] SearchTopicViewModel condition)
        {
            #region Parameters validation

            if (condition == null)
            {
                condition = new SearchTopicViewModel();
                TryValidateModel(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            var loadTopicsResult = await _topicDomain.SearchTopicsAsync(condition, CancellationToken.None);
            return Ok(loadTopicsResult);
        }

        #endregion
    }
}