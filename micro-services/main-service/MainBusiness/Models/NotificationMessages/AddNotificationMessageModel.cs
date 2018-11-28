namespace MainBusiness.Models.NotificationMessages
{
    public class AddNotificationMessageModel<T>
    {
        #region Properties

        /// <summary>
        ///     Notification message owner.
        /// </summary>
        public int OwnerId { get; set; }

        public T ExtraInfo { get; set; }

        public string Message { get; set; }

        #endregion

        #region Constructor

        public AddNotificationMessageModel()
        {
        }

        public AddNotificationMessageModel(int ownerId, T extraInfo, string message)
        {
            OwnerId = ownerId;
            ExtraInfo = extraInfo;
            Message = message;
        }

        #endregion
    }
}