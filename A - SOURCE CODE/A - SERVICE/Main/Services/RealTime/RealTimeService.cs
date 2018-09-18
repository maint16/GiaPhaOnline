using System;
using System.Threading;
using System.Threading.Tasks;
using AppDb.Interfaces;
using Main.Hubs;
using Main.Interfaces.Services;
using Main.Interfaces.Services.RealTime;
using Main.Models.RealTime;
using Microsoft.AspNetCore.SignalR;

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
            _fcmService = fcmService;
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
        private readonly ICloudMessagingService _fcmService;

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
        public async Task SendToClientsAsync<T>(string[] clientIds, string eventName,
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
        public async Task SendToGroupsAsync<T>(string[] groups, string eventName,
            T message,
            CancellationToken cancellationToken)
        {
            await _notificationHubContext
                .Clients
                .Groups(groups)
                .SendAsync(eventName, message, cancellationToken);
        }

        #endregion
    }
}