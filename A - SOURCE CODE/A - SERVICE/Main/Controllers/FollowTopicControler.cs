using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDb.Interfaces;
using AppDb.Models.Entities;
using AppModel.Enumerations;
using AppModel.Enumerations.Order;
using AutoMapper;
using Main.Interfaces.Services;
using Main.ViewModels.FollowCategory;
using Main.ViewModels.FollowTopic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Services;
using Shared.Models;
using Shared.Resources;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Main.Controllers
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
        /// <param name="identityService"></param>
        /// <param name="timeService"></param>
        /// <param name="databaseFunction"></param>
        public FollowTopicControler(IUnitOfWork unitOfWork, IMapper mapper, IIdentityService identityService,
            ITimeService timeService, IRelationalDbService databaseFunction)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _identityService = identityService;
            _timeService = timeService;
            _databaseFunction = databaseFunction;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Instance which is for accessing to database.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        ///     Instance which is for accessing automapper.
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        ///     Instance which is for accessing identity attached in request.
        /// </summary>
        private readonly IIdentityService _identityService;

        /// <summary>
        ///     Service which is for time calculation.
        /// </summary>
        private readonly ITimeService _timeService;

        /// <summary>
        ///     Instance to access generic database function.
        /// </summary>
        private readonly IRelationalDbService _databaseFunction;

        #endregion

        #region Methods

        /// <summary>
        /// Start following a topic.
        /// </summary>
        /// <param name="topicId"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<IActionResult> FollowTopic([FromQuery] int topicId)
        {
            #region Find topic

            // Find topics.
            var topics = _unitOfWork.Topics.Search();
            topics = topics.Where(x => x.Id == topicId && x.Status == ItemStatus.Active);

            // Find the first matched result.
            var topic = await topics.FirstOrDefaultAsync();
            if (topic == null)
                return NotFound(new ApiResponse(HttpMessages.TopicNotFound));

            #endregion

            #region Check whether user already followed topic or not

            // Find request identity.
            var identity = _identityService.GetProfile(HttpContext);

            // Find follow topics.
            var followTopics = _unitOfWork.FollowTopics.Search();
            followTopics = followTopics.Where(x => x.TopicId == topicId && x.FollowerId == identity.Id);
            var followTopic = await followTopics.FirstOrDefaultAsync();

            #endregion

            #region Follow topic initalization

            // Already followed the category.
            if (followTopic != null)
                followTopic.Status = FollowStatus.Following;
            else
            {
                // Initialize follow category.
                followTopic = new FollowTopic();
                followTopic.FollowerId = identity.Id;
                followTopic.TopicId = topicId;
                followTopic.Status = FollowStatus.Following;
                followTopic.CreatedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

                // Insert to system.
                _unitOfWork.FollowTopics.Insert(followTopic);
            }

            // Commit changes.
            await _unitOfWork.CommitAsync();

            #endregion

            return Ok(followTopic);
        }

        /// <summary>
        /// Stop following a topic.
        /// </summary>
        /// <param name="topicId"></param>
        /// <returns></returns>
        [HttpDelete("")]
        public async Task<IActionResult> StopFollowingTopic([FromRoute] int topicId)
        {
            // Find request identity.
            var identity = _identityService.GetProfile(HttpContext);

            // Find topics by using specific conditions.
            var followTopics = _unitOfWork.FollowTopics.Search();
            followTopics = followTopics.Where(x => x.TopicId == topicId && x.FollowerId == identity.Id);

            // Find the first matched.
            var followCategory = await followTopics.FirstOrDefaultAsync();
            if (followCategory == null)
                return NotFound(new ApiResponse(HttpMessages.FollowTopicNotFound));

            // Stop following topic.
            followCategory.Status = FollowStatus.Ignore;

            // Save changes.
            await _unitOfWork.CommitAsync();

            return Ok();
        }

        /// <summary>
        /// Search for following topic by using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("search")]
        public async Task<IActionResult> SearchForFollowingTopics([FromBody] SearchFollowTopicViewModel condition)
        {
            #region Parameters validation

            if (condition == null)
            {
                condition = new SearchFollowTopicViewModel();
                TryValidateModel(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Search for information

            // Find identity in request.
            var identity = _identityService.GetProfile(HttpContext);

            // Search for follow topics.
            var followTopics = _unitOfWork.FollowTopics.Search();

            // Topic id is defined.
            if (condition.TopicIds != null && condition.TopicIds.Count > 0)
            {
                var topicIds = condition.TopicIds.Where(x => x > 0).ToList();
                if (topicIds.Count > 0)
                    followTopics = followTopics.Where(x => condition.TopicIds.Contains(x.TopicId));
            }

            // Search conditions which are based on roles.
            if (identity.Role == AccountRole.Admin)
            {
                // Follower id is defined.
                if (condition.FollowerIds != null && condition.FollowerIds.Count > 0)
                {
                    var followerIds = condition.FollowerIds.Where(x => x > 0).ToList();
                    if (followerIds.Count > 0)
                        followTopics = followTopics.Where(x => condition.FollowerIds.Contains(x.FollowerId));
                }

                // Statuses have been defined.
                if (condition.Statuses != null && condition.Statuses.Count > 0)
                {
                    condition.Statuses =
                      condition.Statuses.Where(x => Enum.IsDefined(typeof(ItemStatus), x)).ToList();
                    if (condition.Statuses.Count > 0)
                        followTopics = followTopics.Where(x => condition.Statuses.Contains(x.Status));
                }
            }
            else
            {
                // Normal users can his/her followed categories.
                followTopics = followTopics.Where(x => x.FollowerId == identity.Id);
            }

            // Created time has been defined.
            var createdTime = condition.CreatedTime;
            if (createdTime != null)
            {
                var from = createdTime.From;
                var to = createdTime.To;

                if (from != null)
                    followTopics = _databaseFunction.SearchNumericProperty(followTopics, x => x.CreatedTime, from.Value,
                        NumericComparision.GreaterEqual);

                if (to != null)
                    followTopics = _databaseFunction.SearchNumericProperty(followTopics, x => x.CreatedTime, to.Value,
                        NumericComparision.LowerEqual);
            }

            // Sort property & direction.
            var sort = condition.Sort;
            if (sort != null)
                followTopics = _databaseFunction.Sort(followTopics, sort.Direction, sort.Property);
            else
                followTopics = _databaseFunction.Sort(followTopics, SortDirection.Decending, FollowTopicSort.CreatedTime);

            var result = new SearchResult<IList<FollowTopic>>();
            result.Total = await followTopics.CountAsync();
            result.Records = await _databaseFunction.Paginate(followTopics, condition.Pagination).ToListAsync();

            #endregion

            return Ok(result);
        }

        #endregion
    }
}
