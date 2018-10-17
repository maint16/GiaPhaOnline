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
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ServiceShared.Exceptions;
using ServiceShared.Interfaces.Services;
using Shared.Enumerations;
using Shared.Enumerations.Order;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.Reply;

namespace AppBusiness.Domain
{
    public class TopicReplyDomain : IReplyDomain
    {
        #region Constructor

        public TopicReplyDomain(IUnitOfWork unitOfWork, IRelationalDbService relationalDbService,
            IHttpContextAccessor httpContextAccessor, IProfileService identityService, ITimeService timeService)
        {
            _unitOfWork = unitOfWork;
            _relationalDbService = relationalDbService;
            _httpContext = httpContextAccessor.HttpContext;
            _identityService = identityService;
            _timeService = timeService;
        }

        #endregion

        #region Properties

        private readonly IUnitOfWork _unitOfWork;

        private readonly IRelationalDbService _relationalDbService;

        private readonly HttpContext _httpContext;

        private readonly IProfileService _identityService;

        private readonly ITimeService _timeService;

        #endregion

        #region Methods

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<Reply> AddReplyAsync(AddReplyViewModel model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Find all topics.
            var topics = _unitOfWork.Topics.Search();
            topics = topics.Where(x => x.Id == model.TopicId && x.Status == ItemStatus.Active);

            // Check whether topic exists or not.
            var topic = await topics.FirstOrDefaultAsync(cancellationToken);
            if (topic == null)
                throw new ApiException(HttpMessages.TopicNotFound, HttpStatusCode.NotFound);

            // Find identity from request.
            var profile = _identityService.GetProfile();

            using (var transaction = _unitOfWork.BeginTransactionScope())
            {
                try
                {
                    #region Add reply

                    // Reply intialization.
                    var reply = new Reply();

#if USE_IN_MEMORY
            var replies = _unitOfWork.Replies.Search();
            var iMaxReplyId = await replies.OrderByDescending(x => x.Id).Select(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);
            reply.Id = iMaxReplyId + 1;
#endif
                    reply.OwnerId = profile.Id;
                    reply.TopicId = topic.Id;
                    reply.CategoryId = topic.CategoryId;
                    reply.CategoryGroupId = topic.CategoryGroupId;
                    reply.Content = model.Content;
                    reply.Status = ItemStatus.Active;
                    reply.CreatedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);
                    reply.LastModifiedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

                    // Insert reply into system.
                    _unitOfWork.Replies.Insert(reply);

                    #endregion

                    #region Update topic summary

                    var topicSummary = await _unitOfWork.TopicSummaries.Search(x => x.TopicId == reply.TopicId).FirstOrDefaultAsync(cancellationToken);
                    if (topicSummary == null)
                    {
                        topicSummary = new TopicSummary(reply.TopicId, 0, 1);
                        _unitOfWork.TopicSummaries.Insert(topicSummary);
                    }
                    else
                    {
                        topicSummary.TotalReply++;
                    }

                    #endregion

                    await _unitOfWork.CommitAsync(cancellationToken);
                    transaction.Commit();
                    return reply;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<Reply> EditReplyAsync(int id, EditReplyViewModel model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Get request identity.
            var profile = _identityService.GetProfile();

            // Get all replies in database.
            var replies = _unitOfWork.Replies.Search();

            replies = replies.Where(x => x.Id == id && x.Status == ItemStatus.Active);

            // Get the first matched reply.
            var reply = await replies.FirstOrDefaultAsync(cancellationToken);
            if (reply == null)
                throw new ApiException(HttpMessages.ReplyNotFound, HttpStatusCode.NotFound);

            // Check whether information has been updated or not.
            var bHasInformationChanged = false;

            // Content is defined
            if (model.Content != null && model.Content != reply.Content)
            {
                reply.Content = model.Content;
                bHasInformationChanged = true;
            }
            
            if (!bHasInformationChanged)
                throw new NotModifiedException();

            reply.LastModifiedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Commit changes to database.
            await _unitOfWork.CommitAsync(cancellationToken);
            return reply;
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<SearchResult<IList<Reply>>> SearchRepliesAsync(SearchReplyViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Get topic replies using specific conditions.
            var topicReplies = GetTopicReplies(condition);

            // Sort by properties.
            if (condition.Sort != null)
                topicReplies =
                    _relationalDbService.Sort(topicReplies, condition.Sort.Direction,
                        condition.Sort.Property);
            else
                topicReplies = _relationalDbService.Sort(topicReplies, SortDirection.Decending,
                    ReplySort.Id);

            // Result initialization.
            var loadTopicRepliesResult = new SearchResult<IList<Reply>>();
            loadTopicRepliesResult.Total = await topicReplies.CountAsync(cancellationToken);
            loadTopicRepliesResult.Records = await _relationalDbService.Paginate(topicReplies, condition.Pagination)
                .ToListAsync(cancellationToken);
            return loadTopicRepliesResult;
        }

        /// <summary>
        ///     Get topic replies using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected virtual IQueryable<Reply> GetTopicReplies(SearchReplyViewModel condition)
        {
            // Get all reply
            var replies = _unitOfWork.Replies.Search();

            // Get profile of requester.
            var profile = _identityService.GetProfile();

            // Id have been defined.
            var ids = condition.Ids;
            if (ids != null && ids.Count > 0)
            {
                ids = ids.Where(x => x > 0).ToHashSet();
                if (ids != null && ids.Count > 0)
                    replies = replies.Where(x => ids.Contains(x.Id));
            }

            // Topic Id have been defined.
            var topicIds = condition.TopicIds;
            if (topicIds != null && topicIds.Count > 0)
            {
                topicIds = topicIds.Where(x => x > 0).ToHashSet();
                if (topicIds != null && topicIds.Count > 0)
                    replies = replies.Where(x => topicIds.Contains(x.TopicId));
            }

            // Category id have been defined
            var categoryIds = condition.CategoryIds;
            if (categoryIds != null && categoryIds.Count > 0)
            {
                categoryIds = categoryIds.Where(x => x > 0).ToHashSet();
                if (categoryIds != null && categoryIds.Count > 0)
                    replies = replies.Where(x => categoryIds.Contains(x.CategoryId));
            }

            // Category group Id have been defined.
            var categoryGroupIds = condition.CategoryGroupIds;
            if (categoryGroupIds != null && categoryGroupIds.Count > 0)
            {
                categoryGroupIds = categoryGroupIds.Where(x => x > 0).ToHashSet();
                if (categoryGroupIds != null && categoryGroupIds.Count > 0)
                    replies = replies.Where(x => categoryGroupIds.Contains(x.CategoryGroupId));
            }

            // Owner Id have been defined.
            var ownerIds = condition.OwnerIds;
            if (ownerIds != null && ownerIds.Count > 0)
            {
                ownerIds = ownerIds.Where(x => x > 0).ToHashSet();
                if (ownerIds != null && ownerIds.Count > 0)
                    replies = replies.Where(x => ownerIds.Contains(x.OwnerId));
            }

            // Content have been defined.
            var content = condition.Contents;
            if (content != null && content.Count > 0)
            {
                content = content.Where(x => !string.IsNullOrWhiteSpace(x)).ToHashSet();
                if (content != null && content.Count > 0)
                    replies = replies.Where(x => content.Any(y => x.Content.Contains(y)));
            }

            // Search conditions which are based on roles.

            if (profile != null && profile.Role == UserRole.Admin)
            {
                var statuses = condition.Statuses;
                if (statuses != null && statuses.Count > 0)
                {
                    statuses =
                        statuses.Where(x => Enum.IsDefined(typeof(ItemStatus), x)).ToHashSet();
                    if (statuses != null && statuses.Count > 0)
                        replies = replies.Where(x => condition.Statuses.Contains(x.Status));
                }
            }
            else
            {
                replies = replies.Where(x =>
                    x.Status == ItemStatus.Active || x.Status == ItemStatus.Disabled && x.OwnerId == profile.Id);
            }

            return replies;
        }

        #endregion
    }
}