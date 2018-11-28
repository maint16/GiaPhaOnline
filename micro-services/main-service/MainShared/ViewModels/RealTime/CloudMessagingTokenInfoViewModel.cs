namespace MainShared.ViewModels.RealTime
{
    public class CloudMessagingTokenInfoViewModel
    {
        #region Properties

        /// <summary>
        ///     package name associated with the token
        /// </summary>
        public string Application { get; set; }

        /// <summary>
        ///     projectId authorized to send to the token
        /// </summary>
        public string AuthorizedEntity { get; set; }

        /// <summary>
        ///     version of the application
        /// </summary>
        public string ApplicationVersion { get; set; }

        /// <summary>
        ///     returns ANDROID, IOS, or CHROME to indicate the device platform to which the token belongs
        /// </summary>
        public string Platform { get; set; }

        #endregion
    }
}