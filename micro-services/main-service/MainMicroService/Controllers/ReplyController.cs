using System.Threading.Tasks;
using AutoMapper;
using MainBusiness.Interfaces;
using MainBusiness.Interfaces.Domains;
using MainBusiness.Models.NotificationMessages;
using MainDb.Interfaces;
using MainMicroService.Constants;
using MainMicroService.Interfaces.Services;
using MainMicroService.Models.AdditionalMessageInfo.Topic;
using MainShared.Resources;
using MainShared.ViewModels.Reply;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServiceShared.Authentications.ActionFilters;
using ServiceShared.Interfaces.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MainMicroService.Controllers
{
    [Route("api/[controller]")]
    public class ReplyController : Controller
    {
        #region Constructors

        public ReplyController(
            IAppUnitOfWork unitOfWork,
            IMapper mapper,
            IBaseTimeService baseTimeService,
            IAppProfileService profileService,
            ISendMailService sendMailService,
            IEmailCacheService emailCacheService,
            ILogger<ReplyController> logger,
            IReplyDomain replyDomain,
            ITopicDomain topicDomain,
            IAppProfileService appProfileService,
            INotificationMessageDomain notificationMessageDomain)
        {
            _sendMailService = sendMailService;
            _emailCacheService = emailCacheService;
            _logger = logger;
            _replyDomain = replyDomain;
            _topicDomain = topicDomain;
            _appProfileService = appProfileService;
            _notificationMessageDomain = notificationMessageDomain;
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

        private readonly IAppProfileService _appProfileService;

        private readonly IReplyDomain _replyDomain;

        private readonly ITopicDomain _topicDomain;

        /// <summary>
        ///     Notification message
        /// </summary>
        private readonly INotificationMessageDomain _notificationMessageDomain;

        #endregion

        #region Methods

        /// <summary>
        ///     Add reply to system.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<IActionResult> AddReply([FromBody] AddReplyViewModel model)
        {
            #region Parameters validation

            if (model == null)
            {
                model = new AddReplyViewModel();
                TryValidateModel(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            var topicReply = await _replyDomain.AddReplyAsync(model);

            // Get requester profile.
            var profile = _appProfileService.GetProfile();

            var topic = _topicDomain.GetTopicUsingIdAsync(model.TopicId);

            #region Notification

            var additionalInfo = new ReplyTopicAdditionalInfoModel();
            additionalInfo.TopicName = topic.Result.Title;
            additionalInfo.ReplierName = profile.Nickname;
            await _notificationMessageDomain.AddNotificationMessageAsync(
                new AddNotificationMessageModel<ReplyTopicAdditionalInfoModel>(topic.Result.OwnerId, additionalInfo,
                    NotificationMessages.SomeoneRepliedYourTopic));

            #endregion

            return Ok(topicReply);
        }

        /// <summary>
        ///     Edit reply by using specific information.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditReply([FromRoute] int id, [FromBody] EditReplyViewModel info)
        {
            #region Parameters validation

            if (info == null)
            {
                info = new EditReplyViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            // Update reply information.
            var reply = await _replyDomain.EditReplyAsync(id, info);
            return Ok(reply);
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
            var deleteReplyViewModel = new DeleteReplyViewModel
            {
                Id = id
            };

            await _replyDomain.DeleteReplyAsync(deleteReplyViewModel);

            return Ok();
        }

        /// <summary>
        ///     Load reply by using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("search")]
        [ByPassAuthorization]
        public async Task<IActionResult> LoadReplies([FromBody] SearchReplyViewModel condition)
        {
            #region Parameters validation

            if (condition == null)
            {
                condition = new SearchReplyViewModel();
                TryValidateModel(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            var loadTopicRepliesResult = await _replyDomain.SearchRepliesAsync(condition);
            return Ok(loadTopicRepliesResult);
        }

        #endregion
    }
}