using System.Threading.Tasks;
using AutoMapper;
using MainBusiness.Interfaces;
using MainBusiness.Interfaces.Domains;
using MainBusiness.Models.NotificationMessages;
using MainDb.Interfaces;
using MainMicroService.Models.AdditionalMessageInfo.Topic;
using MainShared.Resources;
using MainShared.ViewModels.FollowTopic;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MainMicroService.Controllers
{
    [Route("api/follow-topic")]
    public class FollowTopicControler : Controller
    {
        #region Constructors

        /// <summary>
        ///     Initialize controller with injectors.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="mapper"></param>
        /// <param name="profileService"></param>
        /// <param name="followTopicDomain"></param>
        /// <param name="topicDomain"></param>
        /// <param name="appProfileService"></param>
        /// <param name="notificationMessageDomain"></param>
        public FollowTopicControler(IAppUnitOfWork unitOfWork,
            IMapper mapper,
            IAppProfileService profileService,
            IFollowTopicDomain followTopicDomain,
            ITopicDomain topicDomain,
            IAppProfileService appProfileService,
            INotificationMessageDomain notificationMessageDomain)
        {
            _followTopicDomain = followTopicDomain;
            _appProfileService = appProfileService;
            _notificationMessageDomain = notificationMessageDomain;
            _topicDomain = topicDomain;
        }

        #endregion

        #region Properties

        private readonly IFollowTopicDomain _followTopicDomain;

        private readonly ITopicDomain _topicDomain;

        private readonly IAppProfileService _appProfileService;

        /// <summary>
        ///     Notification message
        /// </summary>
        private readonly INotificationMessageDomain _notificationMessageDomain;

        #endregion

        #region Methods

        /// <summary>
        ///     Start following a topic.
        /// </summary>
        /// <param name="topicId"></param>
        /// <returns></returns>
        [HttpPost("{topicId}")]
        public async Task<IActionResult> FollowTopic([FromRoute] int topicId)
        {
            var addFollowTopic = new AddFollowTopicViewModel();
            addFollowTopic.TopicId = topicId;
            var followTopic = await _followTopicDomain.AddFollowTopicAsync(addFollowTopic);

            // Get requester profile.
            var profile = _appProfileService.GetProfile();

            var topic = _topicDomain.GetTopicUsingIdAsync(topicId);

            #region Notification

            var additionalInfo = new FollowTopicAdditionalInfoModel();
            additionalInfo.TopicName = topic.Result.Title;
            additionalInfo.FollowerName = profile.Nickname;
            await _notificationMessageDomain.AddNotificationMessageAsync(
                new AddNotificationMessageModel<FollowTopicAdditionalInfoModel>(topic.Result.OwnerId, additionalInfo,
                    NotificationMessages.SomeoneFollowedYourTopic));

            #endregion

            return Ok(followTopic);
        }

        /// <summary>
        ///     Stop following a topic.
        /// </summary>
        /// <param name="topicId"></param>
        /// <returns></returns>
        [HttpDelete("{topicId}")]
        public async Task<IActionResult> StopFollowingTopic([FromRoute] int topicId)
        {
            var deleteFollowTopicModel = new DeleteFollowTopicViewModel();
            deleteFollowTopicModel.TopicId = topicId;
            await _followTopicDomain.DeleteFollowTopicAsync(deleteFollowTopicModel);
            return Ok();
        }

        /// <summary>
        ///     Search for following topic by using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("search")]
        public async Task<IActionResult> SearchForFollowingTopics([FromBody] SearchFollowTopicViewModel condition)
        {
            if (condition == null)
            {
                condition = new SearchFollowTopicViewModel();
                TryValidateModel(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var loadFollowTopicsResult = await _followTopicDomain.SearchFollowTopicsAsync(condition);
            return Ok(loadFollowTopicsResult);
        }

        #endregion
    }
}