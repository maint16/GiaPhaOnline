using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AppShared.ViewModels.RealTime;
using Main.Models.PushNotification;
using Main.Models.PushNotification.Notification;

namespace Main.Interfaces.Services
{
    public interface ICloudMessagingService
    {
        #region Methods

        /// <summary>
        ///     Get device group notification key by using notification key name.
        /// </summary>
        /// <param name="notificationKeyName"></param>
        /// <returns></returns>
        Task<string> GetDeviceGroupNotificationKey(string notificationKeyName);

        /// <summary>
        ///     Add device to group.
        /// </summary>
        /// <param name="deviceIds"></param>
        /// <param name="group"></param>
        /// <param name="cancellationToken"></param>
        Task<HttpResponseMessage> AddDevicesToGroupAsync(string[] deviceIds, string group,
            CancellationToken cancellationToken);

        /// <summary>
        ///     Add device to group.
        /// </summary>
        /// <param name="deviceIds"></param>
        /// <param name="groups"></param>
        /// <param name="cancellationToken"></param>
        Task<IList<HttpResponseMessage>> AddDevicesToGroupsAsync(string[] deviceIds, IList<string> groups,
            CancellationToken cancellationToken);

        /// <summary>
        ///     Delete device from a specific group.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="group"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> DeleteDeviceFromGroupAsync(string deviceId, string group,
            CancellationToken cancellationToken);

        /// <summary>
        ///     Delete device from a specific group.
        /// </summary>
        /// <param name="deviceIds"></param>
        /// <param name="groups"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IList<HttpResponseMessage>> DeleteDevicesFromGroupsAsync(IList<string> deviceIds, IList<string> groups,
            CancellationToken cancellationToken);

        /// <summary>
        ///     Send push notification to a specific device.
        /// </summary>
        /// <param name="fcmMessage"></param>
        /// <param name="cancellationToken"></param>
        Task<HttpResponseMessage> SendAsync<T>(FcmMessage<T> fcmMessage, CancellationToken cancellationToken);

        /// <summary>
        ///     Send notification to
        /// </summary>
        /// <param name="recipientIds"></param>
        /// <param name="notification"></param>
        /// <param name="collapseKey"></param>
        /// <param name="data"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> SendAsync<T>(List<string> recipientIds,
            FcmBaseNotification notification, string collapseKey, T data,
            CancellationToken cancellationToken);

        /// <summary>
        ///     Get cloud messaging token information from client registration id.
        /// </summary>
        /// <param name="idToken"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<CloudMessagingTokenInfoViewModel> GetCloudMessagingTokenInformationAsync(string idToken,
            CancellationToken cancellationToken = default(CancellationToken));

        #endregion
    }
}