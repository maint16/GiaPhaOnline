using System.Threading.Tasks;
using AppDb.Interfaces;
using AutoMapper;
using Main.Interfaces.Services;
using Main.Interfaces.Services.Businesses;
using Main.ViewModels.FollowTopic;
using Microsoft.AspNetCore.Mvc;
using Shared.Interfaces.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Main.Controllers
{
    [Route("api/follow-topic")]
    public class FollowTopicControler : Controller
    {
        #region Properties

        private readonly IFollowTopicService _followTopicService;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initialize controller with injectors.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="mapper"></param>
        /// <param name="identityService"></param>
        /// <param name="timeService"></param>
        /// <param name="databaseFunction"></param>
        /// <param name="followTopicService"></param>
        public FollowTopicControler(IUnitOfWork unitOfWork, IMapper mapper, IIdentityService identityService,
            ITimeService timeService, IRelationalDbService databaseFunction, IFollowTopicService followTopicService)
        {
            _followTopicService = followTopicService;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Start following a topic.
        /// </summary>
        /// <param name="topicId"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<IActionResult> FollowTopic([FromQuery] int topicId)
        {
            var addFollowTopic = new AddFollowTopicViewModel();
            addFollowTopic.TopicId = topicId;
            var followTopic = await _followTopicService.AddFollowTopicAsync(addFollowTopic);

            return Ok(followTopic);
        }

        /// <summary>
        ///     Stop following a topic.
        /// </summary>
        /// <param name="topicId"></param>
        /// <returns></returns>
        [HttpDelete("")]
        public async Task<IActionResult> StopFollowingTopic([FromRoute] int topicId)
        {
            var deleteFollowTopicModel = new DeleteFollowTopicViewModel();
            deleteFollowTopicModel.TopicId = topicId;
            await _followTopicService.DeleteFollowTopicAsync(deleteFollowTopicModel);
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

            var loadFollowTopicsResult = await _followTopicService.SearchFollowTopicsAsync(condition);
            return Ok(loadFollowTopicsResult);
        }

        #endregion
    }
}