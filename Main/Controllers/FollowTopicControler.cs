using System.Threading.Tasks;
using AppBusiness.Interfaces;
using AppBusiness.Interfaces.Domains;
using AppDb.Interfaces;
using AppShared.ViewModels.FollowTopic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ServiceShared.Interfaces.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Main.Controllers
{
    [Route("api/follow-topic")]
    public class FollowTopicControler : Controller
    {
        #region Properties

        private readonly IFollowTopicDomain _followTopicDomain;

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
        /// <param name="followTopicDomain"></param>
        public FollowTopicControler(IAppUnitOfWork unitOfWork, IMapper mapper, IProfileService identityService,
            ITimeService timeService, IBaseRelationalDbService databaseFunction, IFollowTopicDomain followTopicDomain)
        {
            _followTopicDomain = followTopicDomain;
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
            var followTopic = await _followTopicDomain.AddFollowTopicAsync(addFollowTopic);

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