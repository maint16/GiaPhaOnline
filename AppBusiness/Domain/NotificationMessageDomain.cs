﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AppBusiness.Interfaces;
using AppBusiness.Interfaces.Domains;
using AppBusiness.Models.NotificationMessages;
using AppDb.Interfaces;
using AppDb.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ServiceShared.Exceptions;
using ServiceShared.Interfaces.Services;
using Shared.Enumerations;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.NotificationMessage;

namespace AppBusiness.Domain
{
    public class NotificationMessageDomain : INotificationMessageDomain
    {
        #region Properties

        private readonly ITimeService _timeService;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IRelationalDbService _relationalDbService;

        private readonly IProfileService _profileService;

        private readonly HttpContext _httpContext;

        #endregion

        #region Constructor

        public NotificationMessageDomain(ITimeService timeService, IUnitOfWork unitOfWork, IRelationalDbService relationalDbService, IProfileService profileService, IHttpContextAccessor httpContextAccessor)
        {
            _timeService = timeService;
            _unitOfWork = unitOfWork;
            _relationalDbService = relationalDbService;
            _profileService = profileService;
            _httpContext = httpContextAccessor.HttpContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<NotificationMessage> AddNotificationMessageAsync<T>(
            AddNotificationMessageModel<T> model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var notificationMessage = new NotificationMessage();
            notificationMessage.OwnerId = model.OwnerId;
            notificationMessage.ExtraInfo = JsonConvert.SerializeObject(model.ExtraInfo);
            notificationMessage.Message = model.Message;
            notificationMessage.CreatedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            _unitOfWork.NotificationMessages.Insert(notificationMessage);
            await _unitOfWork.CommitAsync(cancellationToken);

            return notificationMessage;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<NotificationMessage> GetNotificationMessageUsingId(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Get profile information.
            var profile = _profileService.GetProfile(_httpContext);
            if (profile == null)
                return null;

            var notificationMessages = _unitOfWork.NotificationMessages.Search();
            var gId = Guid.Parse(id);

            notificationMessages = notificationMessages.Where(x => x.Id == gId && x.OwnerId == profile.Id);
            return notificationMessages.FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<SearchResult<IList<NotificationMessage>>> SearchNotificationMessagesAsync(SearchNotificationMessageViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var notificationmessages = GetNotificationMessages(condition);
            var loadNotificationMessagesResult = new SearchResult<IList<NotificationMessage>>();
            loadNotificationMessagesResult.Total =  await notificationmessages.CountAsync(cancellationToken);
            loadNotificationMessagesResult.Records = await _relationalDbService
                .Paginate(notificationmessages, condition.Pagination).ToListAsync(cancellationToken);

            return loadNotificationMessagesResult;
        }

        /// <summary>
        /// Search for notification messages using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected virtual IQueryable<NotificationMessage> GetNotificationMessages(
            SearchNotificationMessageViewModel condition)
        {
            var notificationMessages = _unitOfWork.NotificationMessages.Search();

            var statuses = condition.Statuses;
            if (statuses != null && statuses.Count > 0)
            {
                statuses = statuses.Where(x => Enum.IsDefined(typeof(NotificationStatus), x)).ToHashSet();
                if (statuses != null && statuses.Count > 0)
                    notificationMessages = notificationMessages.Where(x => statuses.Contains(x.Status));
            }

            var createdTime = condition.CreatedTime;
            if (createdTime != null)
            {
                if (createdTime.From != null)
                    notificationMessages = notificationMessages.Where(x => x.CreatedTime >= createdTime.From);

                if (createdTime.To != null)
                    notificationMessages = notificationMessages.Where(x => x.CreatedTime <= createdTime.To);
            }

            return notificationMessages;
        }
        #endregion
    }
}