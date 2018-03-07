using Newtonsoft.Json;

namespace Main.Models.PushNotification.Notification
{
    public class WebFcmNotification : FcmBaseNotification
    {
        #region Properties

        /// <summary>
        /// The URL to use for the notification's icon.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// The action associated with a user click on the notification.
        /// For all URL values, secure HTTPS is required.
        /// </summary>
        [JsonProperty("click_action")]
        public string ClickAction { get; set; }

        #endregion
    }
}