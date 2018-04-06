using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SystemConstant.Enumerations;
using SystemDatabase.Interfaces;
using SystemDatabase.Models.Entities;
using Main.Interfaces;
using Main.Interfaces.Services;
using Main.Models;
using Main.Models.PushNotification.Notification;
using Microsoft.AspNetCore.SignalR;

namespace Main.Services
{
    public class NotifyService : INotifyService
    {
        #region Properties

        /// <summary>
        /// Instance to access to database & entities.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Service to send push notification,
        /// </summary>
        private IPushService _pushService;

        /// <summary>
        /// Service to send realtime data.
        /// </summary>
        private IRealTimeNotificationService _realTimeNotificationService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize service with injectors.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="pushService"></param>
        /// <param name="realTimeNotificationService"></param>
        public NotifyService(IUnitOfWork unitOfWork, IPushService pushService, IRealTimeNotificationService realTimeNotificationService)
        {
            _unitOfWork = unitOfWork;
            _pushService = pushService;
            _realTimeNotificationService = realTimeNotificationService;
        }

        #endregion

        #region Properties

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="hubContext"></param>
        /// <param name="roles"></param>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task[] NotifyClients<THub>(IHubContext<THub> hubContext, AccountRole[] roles, string title, string message,
            string eventName, Dictionary<string, object> data) where THub : Hub
        {
            // Get all users from system.
            var users = _unitOfWork.Accounts.Search();

            // Search for signalr connections.
            var signalrConnections = _unitOfWork.SignalrConnections.Search();

            // Search for devices.
            var devices = _unitOfWork.Devices.Search();
            devices = devices.Where(x => x.OwnerId != null);

            var designatedDevices = (from user in users
                                     from device in devices
                                     where user.Id == device.OwnerId
                                     select device.Id).ToList();

            var designatedConnections = (from user in users
                                         from signalrConnection in signalrConnections
                                         where user.Id == signalrConnection.OwnerId
                                         select signalrConnection.Id).ToList();

            #region Notification messages initialization

            var tasks = new List<Task>();
            if (hubContext != null)
            {
                if (designatedDevices.Count > 0)
                {
                    var fcm = new WebFcmNotification();
                    fcm.Body = message;
                    fcm.Title = title;
                    var fcmTask =_pushService.SendNotification(designatedDevices, fcm, eventName, data, CancellationToken.None);

                    tasks.Add(fcmTask);
                }

                if (designatedConnections.Count > 0)
                {
                    var signalrTask = _realTimeNotificationService.BroadcastAsync(hubContext, designatedConnections, eventName, data);

                    tasks.Add(signalrTask);
                }
            }

            #endregion

            return tasks.ToArray();
        }
    }

    #endregion
}