using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemConstant.Enumerations;
using SystemDatabase.Interfaces;
using Main.Interfaces;
using Main.Interfaces.Services;
using Main.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

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
        /// <param name="data"></param>
        /// <returns></returns>
        public Task[] NotifyClients(IHubContext<Hub> hubContext, AccountRole[] roles, string title, string message, Dictionary<string, string> data)
        {
            // Get all users from system.
            var users = _unitOfWork.Accounts.Search();

            // Search for signalr connections.
            var signalrConnections = _unitOfWork.SignalrConnections.Search();

            // Search for devices.
            var devices = _unitOfWork.Devices.Search();
            devices = devices.Where(x => x.OwnerId != null);

            //// Get all connection that need to be broadcasted.
            //var result = from user in users
            //             join signalrConnection in signalrConnections on user.Id equals signalrConnection.OwnerId
            //                 into signalConnectionGroupItems
            //             from signalrConnectionGroupItem in signalConnectionGroupItems
            //             join device in devices on signalrConnectionGroupItem.OwnerId equals device.OwnerId
            //             group signalrConnectionGroupItem by new { signalrConnectionGroupItem.Id  }
            //             orderby last.Key.Month, last.Key.Year
            //             select new
            //             {
            //                 name = last.Key.FirstName + ' ' + last.Key.LastName + " has " +
            //                        last.Sum(m => m.BonusHour).ToString() + "Hr for " +
            //                        Numerictomonth(last.Key.Month) + ' ' + last.Key.Year.ToString()
            //             };


            //#region Signalr messages initialization

            //if (hubContext != null && userConnections.)

            //#endregion

            return null;
        }
    }

    #endregion
}