namespace MainDb.Models.Entities
{
    public class UserDeviceToken
    {
        #region Navigation property

        public virtual User User { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Unique identifier of the device group. This value is returned in the response for a successful create operation,
        ///     and is required for all subsequent operations on the device group
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        ///     The user-defined name of the device group to create or modify
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        ///     Time when device token is created.
        /// </summary>
        public double CreatedTime { get; set; }

        #endregion
    }
}