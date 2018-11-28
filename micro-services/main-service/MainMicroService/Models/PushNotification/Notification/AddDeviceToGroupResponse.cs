using Newtonsoft.Json;

namespace MainMicroService.Models.PushNotification.Notification
{
    public class AddDeviceToGroupResponse
    {
        #region Properties

        /// <summary>
        ///     Notification key returned from service api.
        /// </summary>
        [JsonProperty("notification_key")]
        public string NotificationKey { get; set; }

        #endregion
    }
}