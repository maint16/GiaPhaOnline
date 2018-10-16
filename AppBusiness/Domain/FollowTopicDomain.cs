using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AppBusiness.Interfaces;
using AppBusiness.Interfaces.Domains;
using AppDb.Interfaces;
using AppDb.Models.Entities;
using AppModel.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ServiceShared.Interfaces.Services;
using Shared.Enumerations;
using Shared.Enumerations.Order;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.FollowTopic;

namespace AppBusiness.Domain
{
    public class FollowTopicDomain : IFollowTopicDomain
    {
        #region Constructors

        public FollowTopicDomain(IUnitOfWork unitOfWork, ITimeService timeService,
            IRelationalDbService relationalDbService, IHttpContextAccessor httpContextAccessor,
            IProfileService identityService)
        {
            _unitOfWork = unitOfWork;
            _timeService = timeService;
            _relationalDbService = relationalDbService;
            _httpContext = httpContextAccessor.HttpContext;
            _identityService = identityService;
        }

        #endregion

        #region Properties

        private readonly IUnitOfWork _unitOfWork;

        private readonly ITimeService _timeService;

        private readonly IRelationalDbService _relationalDbService;

        private readonly HttpContext _httpContext;

        private readonly IProfileService _identityService;

        #endregion

        #region Methods

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<FollowTopic> AddFollowTopicAsync(AddFollowTopicViewModel model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Get profile.
            var profile = _identityService.GetProfile(_httpContext);

            // Find topics.
            var topics = _unitOfWork.Topics.Search();
            topics = topics.Where(x => x.Id == model.TopicId && x.Status == ItemStatus.Active);

            // Find the first matched result.
            var topic = await topics.FirstOrDefaultAsync(cancellationToken);
            if (topic == null)
                throw new ApiException(HttpMessages.TopicNotFound, HttpStatusCode.NotFound);

            // Find follow topics.
            var followTopics = _unitOfWork.FollowingTopics.Search();
            followTopics = followTopics.Where(x => x.TopicId == model.TopicId && x.FollowerId == profile.Id);
            var followTopic = await followTopics.FirstOrDefaultAsync(cancellationToken);

            // Already followed the category.
            if (followTopic != null)
            {
                followTopic.Status = FollowStatus.Following;
            }
            else
            {
                // Initialize follow category.
                followTopic = new FollowTopic();
                followTopic.FollowerId = profile.Id;
                followTopic.TopicId = model.TopicId;
                followTopic.Status = FollowStatus.Following;
                followTopic.CreatedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

                // Insert to system.
                _unitOfWork.FollowingTopics.Insert(followTopic);
            }

            // Commit changes.
            await _unitOfWork.CommitAsync(cancellationToken);
            return followTopic;
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task DeleteFollowTopicAsync(DeleteFollowTopicViewModel model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Find request identity.
            var profile = _identityService.GetProfile(_httpContext);

            // Find topics by using specific conditions.
            var followTopics = _unitOfWork.FollowingTopics.Search();
            followTopics = followTopics.Where(x => x.TopicId == model.TopicId && x.FollowerId == profile.Id);

            // Find the first matched.
            var followCategory = await followTopics.FirstOrDefaultAsync(cancellationToken);
            if (followCategory == null)
                throw new ApiException(HttpMessages.FollowTopicNotFound, HttpStatusCode.NotFound);

            // Stop following topic.
            followCategory.Status = FollowStatus.Ignore;

            // Save changes.
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<SearchResult<IList<FollowTopic>>> SearchFollowTopicsAsync(
            SearchFollowTopicViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Get users.
            var followTopics = GetFollowTopics(condition);

            // Sort property & direction.
            var sort = condition.Sort;
            if (sort != null)
                followTopics = _relationalDbService.Sort(followTopics, sort.Direction, sort.Property);
            else
                followTopics =
                    _relationalDbService.Sort(followTopics, SortDirection.Decending, FollowTopicSort.CreatedTime);

            var loadFollowTopicsResult = new SearchResult<IList<FollowTopic>>();
            loadFollowTopicsResult.Total = await followTopics.CountAsync(cancellationToken);
            loadFollowTopicsResult.Records = await _relationalDbService.Paginate(followTopics, condition.Pagination)
                .ToListAsync(cancellationToken);

            return loadFollowTopicsResult;
        }

        /// <summary>
        ///     Get following topics.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected virtual IQueryable<FollowTopic> GetFollowTopics(SearchFollowTopicViewModel condition)
        {
            // Find identity in request.
            var profile = _identityService.GetProfile(_httpContext);

            // Search for follow topics.
            var followTopics = _unitOfWork.FollowingTopics.Search();

            // Topic id is defined.
            var topicIds = condition.TopicIds;
            if (topicIds != null && topicIds.Count > 0)
            {
                topicIds = topicIds.Where(x => x > 0).ToHashSet();
                if (topicIds.Count > 0)
                    followTopics = followTopics.Where(x => topicIds.Contains(x.TopicId));
            }

            // Search conditions which are based on roles.
            if (profile != null && profile.Role == UserRole.Admin)
            {
                // Follower id is defined.
                var followerIds = condition.FollowerIds;
                if (followerIds != null && followerIds.Count > 0)
                {
                    followerIds = condition.FollowerIds.Where(x => x > 0).ToHashSet();
                    if (followerIds.Count > 0)
                        followTopics = followTopics.Where(x => followerIds.Contains(x.FollowerId));
                }

                // Statuses have been defined.
                var statuses = condition.Statuses;
                if (statuses != null && statuses.Count > 0)
                {
                    statuses =
                        statuses.Where(x => Enum.IsDefined(typeof(ItemStatus), x)).ToHashSet();
                    if (statuses.Count > 0)
                        followTopics = followTopics.Where(x => statuses.Contains(x.Status));
                }
            }
            else
            {
                // Normal users can his/her followed categories.
                followTopics = followTopics.Where(x => x.FollowerId == profile.Id);
            }

            // Created time has been defined.
            var createdTime = condition.CreatedTime;
            if (createdTime != null)
            {
                var from = createdTime.From;
                var to = createdTime.To;

                if (from != null)
                    followTopics = _relationalDbService.SearchNumericProperty(followTopics, x => x.CreatedTime,
                        from.Value,
                        NumericComparision.GreaterEqual);

                if (to != null)
                    followTopics = _relationalDbService.SearchNumericProperty(followTopics, x => x.CreatedTime,
                        to.Value,
                        NumericComparision.LowerEqual);
            }

            return followTopics;
        }

        #endregion
    }
}