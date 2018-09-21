namespace Main.Constants.RealTime
{
    public class FcmErrorMessageConstant
    {
        #region Properties

        /// <summary>
        /// Message which is thrown from api service when notification key is unavailable.
        /// </summary>
        public const string NotificationKeyNotFound = "notification_key not found";

        /// <summary>
        /// Device not registered.
        /// </summary>
        public const string DeviceNotRegistered = "NotRegistered";

        /// <summary>
        /// Token is invalid.
        /// </summary>
        public const string InvalidRegistrationToken = "InvalidRegistration";


        #endregion
    }
}