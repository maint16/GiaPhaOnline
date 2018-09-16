using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppDb.Interfaces;
using AppDb.Models.Entities;
using Main.Hubs;
using Main.Interfaces.Services;
using Main.Interfaces.Services.RealTime;
using Main.Models.PushNotification;
using Main.Models.PushNotification.Notification;
using Main.Models.RealTime;
using Microsoft.AspNetCore.SignalR;

namespace Main.Services.RealTime
{
    public class RealTimeService : IRealTimeService
    {
        #region Properties

        /// <summary>
        /// Notification hub context.
        /// </summary>
        private readonly IHubContext<NotificationHub> _notificationHubContext;

        /// <summary>
        /// Push service.
        /// </summary>
        private readonly ICloudMessagingService _fcmService;

        /// <summary>
        /// Unit of work.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;
        
        #endregion

        #region Constructor

        /// <summary>
        /// Initialize service with injectors.
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

        #region Methods

        /// <summary>
        /// Add client to push group group async.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="groupName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public  Task AddClientToPushGroupAsync(string clientId, string groupName, CancellationToken cancellationToken)
        {
            return _fcmService.AddDeviceToGroupAsync(clientId, groupName, cancellationToken);
        }

        /// <summary>
        /// Add client to real time group async.
        /// </summary>
        /// <param name="clientId"></param
        /// <param name="userId"></param>>
        /// <param name="groups"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task AddClientToRealTimeGroupsAsync(string clientId, int userId, string[] groups, CancellationToken cancellationToken)
        {
            using (var transaction = _unitOfWork.BeginTransactionScope())
            {
                // Remove all previous connection.
                var signalrConnections = _unitOfWork.SignalrConnections.Search();
                signalrConnections = signalrConnections.Where(x => x.ClientId == clientId && x.UserId == userId);
                _unitOfWork.SignalrConnections.Remove(signalrConnections);

                // Remove all previous connection groups.
                var signalrConnectionGroups = _unitOfWork.SignalrConnectionGroups.Search();
                signalrConnectionGroups = signalrConnectionGroups.Where(x =>
                    x.ClientId == clientId && groups.Any(group => group.Equals(x.Group)));
                _unitOfWork.SignalrConnectionGroups.Remove(signalrConnectionGroups);

                // Add connection.
                var signalConnection = new SignalrConnection();
                signalConnection.UserId = userId;
                signalConnection.ClientId = clientId;
                _unitOfWork.SignalrConnections.Insert(signalConnection);

                // Add connection to specific groups.
                foreach (var group in groups)
                {
                    var signalConnectionGroup = new SignalrConnectionGroup();
                    signalConnectionGroup.ClientId = clientId;
                    signalConnectionGroup.Group = group;
                    signalConnectionGroup.UserId = userId;
                    _unitOfWork.SignalrConnectionGroups.Insert(signalConnectionGroup);
                }

                await _unitOfWork.CommitAsync();
                transaction.Commit();
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="eventName"></param>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SendToGroupsAsync<TMessageAdditionalData>(string[] groups, string eventName, RealTimeMessage<TMessageAdditionalData> message, CancellationToken cancellationToken)
        {
            // Initialize background tasks.
            var backgroundTasks = new List<Task>();
            
            // Initialize real time hub message task.
            var pSendRealTimeHubMessageTask = _notificationHubContext.Clients.Groups(groups).SendAsync(eventName, message, cancellationToken);
            backgroundTasks.Add(pSendRealTimeHubMessageTask);
            
            // Initialize firebase cloud messaging.
            var messageContent = new WebFcmMessageContent();
            messageContent.Title = message.Header;
            messageContent.Body = message.Body;

            // Fire cloud messaging.
            var fcmMessage = new FcmMessage<TMessageAdditionalData>();
            fcmMessage.Condition = string.Join("||", groups.Select(x => $"'{x} in topics'"));
            fcmMessage.Data = message.AdditionalInfo;
            var pSendFcmMessageTask = _fcmService.SendAsync(fcmMessage, cancellationToken);
            backgroundTasks.Add(pSendFcmMessageTask);

            await Task.WhenAll(backgroundTasks.ToArray());
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <typeparam name="TMessageAdditionalData"></typeparam>
        /// <param name="clientIds"></param>
        /// <param name="eventName"></param>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SendToClientsAsync<TMessageAdditionalData>(string[] clientIds, string eventName, RealTimeMessage<TMessageAdditionalData> message,
            CancellationToken cancellationToken)
        {
            // Initialize background tasks.
            var backgroundTasks = new List<Task>();

            // Initialize real time hub message task.
            var pSendRealTimeHubMessageTask = _notificationHubContext.Clients.Clients(clientIds).SendAsync(eventName, message, cancellationToken);
            backgroundTasks.Add(pSendRealTimeHubMessageTask);

            // Initialize firebase cloud messaging.
            var messageContent = new WebFcmMessageContent();
            messageContent.Title = message.Header;
            messageContent.Body = message.Body;

            // Fire cloud messaging.
            var fcmMessage = new FcmMessage<TMessageAdditionalData>();
            fcmMessage.Condition = string.Join("||", clientIds.Select(x => $"'{x} in topics'"));
            fcmMessage.Data = message.AdditionalInfo;
            var pSendFcmMessageTask = _fcmService.SendAsync(fcmMessage, cancellationToken);
            backgroundTasks.Add(pSendFcmMessageTask);

            await Task.WhenAll(backgroundTasks.ToArray());
        }
    }

    #endregion
}