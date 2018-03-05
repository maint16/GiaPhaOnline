using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Main.Models.PushNotification;

namespace Main.Interfaces.Services
{
    public interface IPushNotificationService
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
        Task<HttpResponseMessage> SendNotification(FcmMessage fcmMessage);

        #endregion
    }
}