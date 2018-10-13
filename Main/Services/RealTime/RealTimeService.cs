using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AppDb.Interfaces;
using AppDb.Models.Entities;
using Main.Constants.RealTime;
using Main.Hubs;
using Main.Interfaces.Services;
using Main.Interfaces.Services.RealTime;
using Main.Models.PushNotification;
using Main.Models.PushNotification.Notification;
using Main.Models.RealTime;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Shared.Enumerations;
using Shared.ViewModels.RealTime;

namespace Main.Services.RealTime
{
    public class RealTimeService : IRealTimeService
    {
        #region Constructor

        /// <summary>
        ///     Initialize service with injectors.
        /// </summary>
        public RealTimeService(IHubContext<NotificationHub> notificationHubContext,
            ICloudMessagingService fcmService,
            IUnitOfWork unitOfWork)
        {
            _notificationHubContext = notificationHubContext;
            _cloudMessagingService = fcmService;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Notification hub context.
        /// </summary>
        private readonly IHubContext<NotificationHub> _notificationHubContext;

        /// <summary>
        ///     Push service.
        /// </summary>
        private readonly ICloudMessagingService _cloudMessagingService;

        /// <summary>
        ///     Unit of work.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Methods

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="clientIds"></param>
        /// <param name="eventName"></param>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SendRealTimeMessageToClientsAsync<T>(string[] clientIds, string eventName,
            T message,
            CancellationToken cancellationToken)
        {
            if (clientIds == null)
                throw new Exception("NO_CLIENT_SPECIFIED");

            if (string.IsNullOrEmpty(eventName))
                throw new Exception("NO_EVENT_SPECIFIED");

            await _notificationHubContext
                .Clients
                .Clients(clientIds)
                .SendAsync(eventName, message, cancellationToken);
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="groups"></param>
        /// <param name="eventName"></param>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SendRealTimeMessageToGroupsAsync<T>(string[] groups, string eventName,
            T message,
            CancellationToken cancellationToken)
        {
            await _notificationHubContext
                .Clients
                .Groups(groups)
                .SendAsync(eventName, message, cancellationToken);
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task SendPushMessageToGroupsAsync<T>(string[] groups, string collapseKey,
            RealTimeMessage<T> message,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // No group is defined.
            if (groups == null || groups.Length < 1)
                throw new Exception("No group is defined.");

            // Get recipient ids belongs groups.
            var userRealTimeGroups = _unitOfWork.UserRealTimeGroups.Search();
            userRealTimeGroups = userRealTimeGroups.Where(x => groups.Contains(x.Group));

            var userDevices = _unitOfWork.UserDeviceTokens.Search();
            var userDeviceIds = await (from userRealTimeGroup in userRealTimeGroups
                from userDevice in userDevices
                where userRealTimeGroup.UserId == userDevice.UserId
                select userDevice.DeviceId).ToListAsync(cancellationToken);

            if (userDeviceIds == null || userDeviceIds.Count < 1)
                return;

            var firebasePushMessage = new FcmMessage<RealTimeMessage<T>>();
            firebasePushMessage.RegistrationIds = userDeviceIds;
            firebasePushMessage.CollapseKey = collapseKey;
            firebasePushMessage.Data = message;

            var firebaseWebNotification = new WebFcmMessageContent();
            firebaseWebNotification.Icon = message.Icon;
            firebaseWebNotification.Body = message.Body;
            firebaseWebNotification.Title = message.Title;

            var httpResponseMessage = await _cloudMessagingService.SendAsync(firebasePushMessage, cancellationToken);
            await DeleteFailedDeviceAsync(httpResponseMessage, userDeviceIds, cancellationToken);
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public string[] GetUserAvailableRealTimeGroups(User user)
        {
            var groups = new List<string>();

            if (user.Role == UserRole.Admin)
                groups.Add(RealTimeGroupConstant.Admin);

            return groups.ToArray();
        }

        /// <summary>
        ///     Base on the response from FCM service to decide what device should be deleted.
        /// </summary>
        /// <param name="httpResponseMessage"></param>
        /// <param name="originalDeviceIds"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task DeleteFailedDeviceAsync(HttpResponseMessage httpResponseMessage,
            List<string> originalDeviceIds, CancellationToken cancellationToken)
        {
            if (httpResponseMessage == null)
                return;

            var httpContent = httpResponseMessage.Content;
            if (httpContent == null)
                return;

            var pushResponse = await httpContent.ReadAsAsync<FcmPushMessageResponseViewModel>(cancellationToken);
            // If there is at least one failed token, find 'em and delete 'em from device database.
            if (pushResponse.FailedRecipients == 0 || pushResponse.Results == null)
                return;

            var messageResults = pushResponse.Results;
            var failedMessages = new[]
            {
                FcmErrorMessageConstant.DeviceNotRegistered,
                FcmErrorMessageConstant.InvalidRegistrationToken
            };

            var failedIndexes = messageResults.Select((c, i) => new {MessageResult = c, Index = i})
                .Where(x => failedMessages.Contains(x.MessageResult.Error))
                .Select(x => x.Index);

            // Enlist of devices that must be removed.
            var deletedDeviceId = originalDeviceIds.Select((d, i) => new {DeviceId = d, Index = i})
                .Where(x => failedIndexes.Contains(x.Index))
                .Select(x => x.DeviceId);

            var deviceIds = _unitOfWork.UserDeviceTokens.Search();
            deviceIds = deviceIds.Where(x => deletedDeviceId.Contains(x.DeviceId));
            _unitOfWork.UserDeviceTokens.Remove(deviceIds);
            await _unitOfWork.CommitAsync();
        }

        #endregion
    }
}