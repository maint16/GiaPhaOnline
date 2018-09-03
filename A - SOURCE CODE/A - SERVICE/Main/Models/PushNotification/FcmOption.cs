namespace Main.Models.PushNotification
{
    public class FcmOption
    {
        #region Properties

        /// <summary>
        /// Key which is used for submitting request to FCM server.
        /// </summary>
        public string ServerKey { get; set; }
        
        /// <summary>
        /// Id of project.
        /// </summary>
        public string SenderId { get; set; }

        #endregion
    }
}