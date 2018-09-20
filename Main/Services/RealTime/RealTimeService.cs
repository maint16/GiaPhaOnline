using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppDb.Interfaces;
using AppDb.Models.Entities;
using AppModel.Enumerations;
using Main.Constants.RealTime;
using Main.Hubs;
using Main.Interfaces.Services;
using Main.Interfaces.Services.RealTime;
using Main.Models.PushNotification;
using Main.Models.PushNotification.Notification;
using Main.Models.RealTime;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

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
        /// <inheritdoc />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task SendPushMessageToGroupsAsync<T>(string[] groups, string collapseKey, RealTimeMessage<T> message,
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
            
            var firebasePushMessage = new FcmMessage<RealTimeMessage<T>>();
            firebasePushMessage.RegistrationIds = userDeviceIds;
            firebasePushMessage.CollapseKey = collapseKey;
            firebasePushMessage.Data = message;

            var firebaseWebNotification = new WebFcmMessageContent();
            firebaseWebNotification.Icon = message.Icon;
            firebaseWebNotification.Body = message.Body;
            firebaseWebNotification.Title = message.Title;

            await _cloudMessagingService.SendAsync(firebasePushMessage, cancellationToken);
        }

        /// <summary>
        /// <inheritdoc />
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

        #endregion
    }
}