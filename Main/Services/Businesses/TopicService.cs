﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AppDb.Interfaces;
using AppDb.Models.Entities;
using AppDb.Services;
using AppModel.Enumerations;
using AppModel.Enumerations.Order;
using AppModel.Exceptions;
using Main.Interfaces.Services;
using Main.Interfaces.Services.Businesses;
using Main.ViewModels.Topic;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Services;
using Shared.Models;
using Shared.Resources;
using Shared.Services;

namespace Main.Services.Businesses
{
    public class TopicService : ITopicService
    {
        #region Properties

        private readonly IUnitOfWork _unitOfWork;

        private readonly IRelationalDbService _relationalDbService;

        private readonly IIdentityService _identityService;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly ITimeService _timeService;

        private readonly HttpContext _httpContext;

        #endregion

        #region Constructors

        public TopicService(IUnitOfWork unitOfWork, IRelationalDbService relationalDbService, IIdentityService identityService, ITimeService timeService, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _relationalDbService = relationalDbService;
            _identityService = identityService;
            _timeService = timeService;
            _httpContextAccessor = httpContextAccessor;
            _httpContext = httpContextAccessor.HttpContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<Topic> AddTopicAsync(AddTopicViewModel model, CancellationToken cancellationToken = default(CancellationToken))
        {
            #region Find topic

            // Find category.
            var categories = _unitOfWork.Categories.Search();
            categories = categories.Where(x => x.Id == model.CategoryId && x.Status == ItemStatus.Active);

            // Check whether category exists or not.
            var bIsCategoryAvailable = await categories.AnyAsync(cancellationToken);
            if (!bIsCategoryAvailable)
                throw new ApiException(HttpMessages.CategoryNotFound, HttpStatusCode.NotFound);

            #endregion

            #region Topic initialization

            // Find identity from request.
            var profile = _identityService.GetProfile(_httpContextAccessor.HttpContext);

            // Topic intialization.
            var topic = new Topic();

#if USE_IN_MEMORY
            topic.Id = await _unitOfWork.Topics.Search().OrderByDescending(x => x.Id).Select(x => x.Id).FirstOrDefaultAsync(cancellationToken) + 1;
#endif
            topic.OwnerId = profile.Id;
            topic.CategoryId = model.CategoryId;
            topic.CategoryGroupId = model.CategoryGroupId;
            topic.Title = model.Title;
            topic.Body = model.Body;
            topic.Status = ItemStatus.Active;
            topic.CreatedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);
            topic.LastModifiedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Insert topic into system.
            _unitOfWork.Topics.Insert(topic);

            await _unitOfWork.CommitAsync(cancellationToken);

            #endregion

            return topic;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<Topic> EditTopicAsync(int id, EditTopicViewModel model, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Get request identity.
            var profile = _identityService.GetProfile(_httpContext);

            // Get all topics in database.
            var topics = _unitOfWork.Topics.Search();

            topics = topics.Where(x => x.Id == id && x.Status == ItemStatus.Active);

            // Get the first matched topic.
            var topic = await topics.FirstOrDefaultAsync(cancellationToken);
            if (topic == null)
                throw new ApiException(HttpMessages.TopicNotFound, HttpStatusCode.NotFound);

            // Check whether information has been updated or not.
            var bHasInformationChanged = false;

            // Category id is defined
            if (model.CategoryId != topic.CategoryId)
            {
                topic.CategoryId = model.CategoryId;
                bHasInformationChanged = true;
            }

            // Category group id is defined
            if (model.CategoryGroupId != topic.CategoryGroupId)
            {
                topic.CategoryGroupId = model.CategoryGroupId;
                bHasInformationChanged = true;
            }

            // Title is defined
            if (model.Title != null && model.Title != topic.Title)
            {
                topic.Title = model.Title;
                bHasInformationChanged = true;
            }

            // Body is defined
            if (model.Body != null && model.Body != topic.Body)
            {
                topic.Body = model.Body;
                bHasInformationChanged = true;
            }

            // Status is defined.
            if (model.Status != topic.Status)
            {
                topic.Status = model.Status;
                bHasInformationChanged = true;
            }

            if (!bHasInformationChanged)
                return topic;

            topic.LastModifiedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Commit changes to database.
            await _unitOfWork.CommitAsync(cancellationToken);
            return topic;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<SearchResult<IList<Topic>>> SearchTopicsAsync(SearchTopicViewModel condition, CancellationToken cancellationToken)
        {
            var topics = GetTopics(condition);

            // Sort by properties.
            if (condition.Sort != null)
                topics =
                    _relationalDbService.Sort(topics, condition.Sort.Direction,
                        condition.Sort.Property);
            else
                topics = _relationalDbService.Sort(topics, SortDirection.Decending,
                    TopicSort.Title);

            // Result initialization.
            var loadTopicsResult = new SearchResult<IList<Topic>>();
            loadTopicsResult.Total = await topics.CountAsync(cancellationToken);
            loadTopicsResult.Records = await _relationalDbService.Paginate(topics, condition.Pagination).ToListAsync(cancellationToken);
            return loadTopicsResult;
        }

        /// <summary>
        /// Get topics using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected virtual IQueryable<Topic> GetTopics(SearchTopicViewModel condition)
        {
            // Get all topic
            var topics = _unitOfWork.Topics.Search();

            // Id have been defined.
            var ids = condition.Ids;
            if (ids != null && ids.Count > 0)
            {
                ids = ids.Where(x => x > 0).ToHashSet();
                if (condition.Ids != null && condition.Ids.Count > 0)
                    topics = topics.Where(x => ids.Contains(x.Id));
            }

            // Category id have been defined
            var categoryIds = condition.CategoryIds;
            if (categoryIds != null && categoryIds.Count > 0)
            {
                categoryIds = categoryIds.Where(x => x > 0).ToHashSet();
                if (categoryIds != null && categoryIds.Count > 0)
                    topics = topics.Where(x => condition.CategoryIds.Contains(x.CategoryId));
            }

            // Category group Id have been defined.
            var categoryGroupIds = condition.CategoryGroupIds;
            if (categoryGroupIds != null && categoryGroupIds.Count > 0)
            {
                categoryGroupIds = categoryGroupIds.Where(x => x > 0).ToHashSet();
                if (categoryGroupIds != null && categoryGroupIds.Count > 0)
                    topics = topics.Where(x => condition.CategoryGroupIds.Contains(x.CategoryGroupId));
            }

            // Owner Id have been defined.
            var ownerIds = condition.OwnerIds;
            if (ownerIds != null && ownerIds.Count > 0)
            {
                ownerIds = ownerIds.Where(x => x > 0).ToHashSet();
                if (ownerIds != null && ownerIds.Count > 0)
                    topics = topics.Where(x => condition.OwnerIds.Contains(x.OwnerId));
            }

            // Search conditions which are based on roles.
            var profile = _identityService.GetProfile(_httpContext);
            if (profile != null && profile.Role == UserRole.Admin)
            {
                var statuses = condition.Statuses?.Where(x => Enum.IsDefined(typeof(UserRole), x)).ToHashSet();
                if (statuses != null && statuses.Count > 0)
                    topics = topics.Where(x => condition.Statuses.Contains(x.Status));
            }
            else
                topics = topics.Where(x => x.Status == ItemStatus.Active || (x.Status == ItemStatus.Disabled && x.OwnerId == profile.Id));

            return topics;
        }

        #endregion
    }
}