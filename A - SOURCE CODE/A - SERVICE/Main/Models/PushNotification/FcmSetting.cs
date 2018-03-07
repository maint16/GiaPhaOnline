namespace Main.Models.PushNotification
{
    public class FcmSetting
    {
        #region Properties

        /// <summary>
        /// Key which is used for submitting request to FCM server.
        /// </summary>
        public string ServerKey { get; set; }

        /// <summary>
        /// Maximum of devices which notification can be sent to at one time.
        /// </summary>
        public int MaxDevices { get; set; } = 1000;

        #endregion
    }
}