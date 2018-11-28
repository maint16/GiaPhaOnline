namespace Main.Models.PushNotification.Notification
{
    /// <summary>
    ///     Base class of FCM Notification.
    /// </summary>
    public class FcmBaseNotification
    {
        #region Properties

        /// <summary>
        ///     The notification's title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     The notification's body text.
        /// </summary>
        public string Body { get; set; }

        #endregion
    }
}