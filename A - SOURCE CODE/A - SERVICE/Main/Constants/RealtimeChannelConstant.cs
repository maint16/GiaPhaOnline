namespace Main.Constants
{
    public class RealtimeChannelConstant
    {
        #region Properties

        /// <summary>
        /// Channel to send new user registration to admin.
        /// </summary>
        public const string PrivateEventUserRegistered = "private-user_registered";

        /// <summary>
        /// Event that happens when an user takes action on a category.
        /// </summary>
        public const string EventActionOnCategory = "event-action-on-category";

        /// <summary>
        /// Hub to send notification to users
        /// </summary>
        public const string NotificationHubName = "/hub/notification";

        #endregion
    }
}