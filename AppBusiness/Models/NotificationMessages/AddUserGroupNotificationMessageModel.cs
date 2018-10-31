using System.Collections.Generic;

namespace AppBusiness.Models.NotificationMessages
{
    public class AddUserGroupNotificationMessageModel<T>
    {
        #region Properties

        public T ExtraInfo { get; set; }

        public string Message { get; set; }

        public HashSet<int> IgnoredUserIds { get; set; }

        #endregion

        #region Constructor

        public AddUserGroupNotificationMessageModel()
        {
        }

        public AddUserGroupNotificationMessageModel(T extraInfo, string message)
        {
            ExtraInfo = extraInfo;
            Message = message;
        }

        #endregion
    }
}