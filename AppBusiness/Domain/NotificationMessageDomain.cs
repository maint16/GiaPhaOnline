using System;
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
using AppModel.Enumerations;
using AppShared.Resources;
using AppShared.ViewModels.NotificationMessage;
using ClientShared.Enumerations;
using ClientShared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ServiceShared.Exceptions;
using ServiceShared.Interfaces.Services;

namespace AppBusiness.Domain
{
    public class NotificationMessageDomain : INotificationMessageDomain
    {
        #region Constructor

        public NotificationMessageDomain(IBaseTimeService baseTimeService, IAppUnitOfWork unitOfWork,
            IBaseRelationalDbService relationalDbService, IAppProfileService profileService,
            IHttpContextAccessor httpContextAccessor)
        {
            _baseTimeService = baseTimeService;
            _unitOfWork = unitOfWork;
            _relationalDbService = relationalDbService;
            _profileService = profileService;
            _httpContext = httpContextAccessor.HttpContext;
        }

        #endregion

        #region Properties

        private readonly IBaseTimeService _baseTimeService;

        private readonly IAppUnitOfWork _unitOfWork;

        private readonly IBaseRelationalDbService _relationalDbService;

        private readonly IAppProfileService _profileService;

        private readonly HttpContext _httpContext;

        #endregion

        #region Methods

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="bIsExpressionSupressed"></param>
        /// <returns></returns>
        public virtual async Task<NotificationMessage> AddNotificationMessageAsync<T>(
            AddNotificationMessageModel<T> model,
            CancellationToken cancellationToken = default(CancellationToken),
            bool bIsExpressionSupressed = default(bool))
        {
            try
            {
                var notificationMessage = new NotificationMessage();
                notificationMessage.OwnerId = model.OwnerId;
                notificationMessage.ExtraInfo = JsonConvert.SerializeObject(model.ExtraInfo);
                notificationMessage.Message = model.Message;
                notificationMessage.Status = NotificationStatus.Unseen;
                notificationMessage.CreatedTime = _baseTimeService.DateTimeUtcToUnix(DateTime.UtcNow);

                _unitOfWork.NotificationMessages.Insert(notificationMessage);
                await _unitOfWork.CommitAsync(cancellationToken);

                return notificationMessage;
            }
            catch
            {
                if (bIsExpressionSupressed)
                    return null;

                throw;
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="userGroup"></param>
        /// <param name="model"></param>
        /// <param name="bIsExceptionSuppressed"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task AddNotificationMessageToUserGroup<T>(UserGroup userGroup, AddUserGroupNotificationMessageModel<T> model,
            bool bIsExceptionSuppressed = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                // Base on the user group, search for user.
                var users = _unitOfWork.Accounts.Search(x => x.Status == UserStatus.Available);
                switch (userGroup)
                {
                    case UserGroup.Admin:
                        users = users.Where(x => x.Role == UserRole.Admin);
                        break;
                    case UserGroup.User:
                        users = users.Where(x => x.Role == UserRole.User);
                        break;
                }

                var ignoredUserIds = model.IgnoredUserIds;
                if (model.IgnoredUserIds != null)
                {
                    ignoredUserIds = ignoredUserIds.Where(x => x > 0).ToHashSet();
                    if (ignoredUserIds.Count > 0)
                        users = users.Where(x => !ignoredUserIds.Contains(x.Id));
                }

                foreach (var user in users)
                {
                    var notificationMessage = new NotificationMessage();
                    notificationMessage.OwnerId = user.Id;
                    notificationMessage.ExtraInfo = JsonConvert.SerializeObject(model.ExtraInfo);
                    notificationMessage.Message = model.Message;
                    notificationMessage.Status = NotificationStatus.Unseen;
                    notificationMessage.CreatedTime = _baseTimeService.DateTimeUtcToUnix(DateTime.UtcNow);

                    _unitOfWork.NotificationMessages.Insert(notificationMessage);
                }
                
                
                await _unitOfWork.CommitAsync(cancellationToken);
            }
            catch
            {
                if (bIsExceptionSuppressed)
                    return;

                throw;
            }
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<NotificationMessage> GetNotificationMessageUsingId(Guid id,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Get profile information.
            var profile = _profileService.GetProfile();
            if (profile == null)
                return null;

            var notificationMessages = _unitOfWork.NotificationMessages.Search();
            notificationMessages = notificationMessages.Where(x => x.Id == id && x.OwnerId == profile.Id);
            return notificationMessages.FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        ///     Get
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<NotificationMessage> MarkNotificationMessageAsSeen(Guid id,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var notificationMessage = await GetNotificationMessageUsingId(id, cancellationToken);
            if (notificationMessage == null)
                throw new ApiException(HttpStatusCode.NotFound, HttpMessages.NotificationMessageNotFound);

            if (notificationMessage.Status == NotificationStatus.Seen)
                return notificationMessage;

            notificationMessage.Status = NotificationStatus.Seen;
            await _unitOfWork.CommitAsync(cancellationToken);
            return notificationMessage;
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<SearchResult<IList<NotificationMessage>>> SearchNotificationMessagesAsync(
            SearchNotificationMessageViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var notificationmessages = GetNotificationMessages(condition);
            var loadNotificationMessagesResult = new SearchResult<IList<NotificationMessage>>();
            loadNotificationMessagesResult.Total = await notificationmessages.CountAsync(cancellationToken);
            loadNotificationMessagesResult.Records = await _relationalDbService
                .Paginate(notificationmessages, condition.Pagination).ToListAsync(cancellationToken);

            return loadNotificationMessagesResult;
        }

        /// <summary>
        ///     Search for notification messages using specific conditions.
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