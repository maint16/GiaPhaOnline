using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Main.Models.PushNotification;
using Main.Models.PushNotification.Notification;

namespace Main.Interfaces.Services
{
    public interface IPushService
    {
        #region Methods

        /// <summary>
        /// Add device to group.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="group"></param>
        Task<HttpResponseMessage> AddDeviceToGroupAsync(string deviceId, string group);

        /// <summary>
        /// Send push notification to a specific device.
        /// </summary>
        /// <param name="fcmMessage"></param>
        /// <param name="cancellationToken"></param>
        Task<HttpResponseMessage> SendNotification(FcmMessage fcmMessage, CancellationToken cancellationToken);

        /// <summary>
        /// Send notification to 
        /// </summary>
        /// <param name="recipientIds"></param>
        /// <param name="notification"></param>
        /// <param name="collapseKey"></param>
        /// <param name="data"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> SendNotification(List<string> recipientIds,
           FcmBaseNotification notification, string collapseKey, IDictionary data,
           CancellationToken cancellationToken);


        #endregion
    }
}