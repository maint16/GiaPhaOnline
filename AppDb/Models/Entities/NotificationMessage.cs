using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AppShared.Enumerations;
using Newtonsoft.Json;

namespace AppDb.Models.Entities
{
    public class NotificationMessage
    {
        #region Relationships

        /// <summary>
        ///     One notification can only be initiated by one account.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(OwnerId))]
        public User Owner { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Id of notification
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        ///     Owner of notification
        /// </summary>
        public int OwnerId { get; set; }
        
        /// <summary>
        ///     Status of notification
        /// </summary>
        public NotificationStatus Status { get; set; }

        /// <summary>
        ///     When the notification message was created.
        /// </summary>
        public double CreatedTime { get; set; }

        /// <summary>
        ///     Serialized information of notification
        /// </summary>
        public string ExtraInfo { get; set; }

        /// <summary>
        ///     Message of notification
        /// </summary>
        public string Message { get; set; }

        #endregion

        #region Constructor

        public NotificationMessage()
        {
        }

        public NotificationMessage(int ownerId, NotificationStatus status, double createdTime, string extraInfo, string message)
        {
            OwnerId = ownerId;
            Status = status;
            CreatedTime = createdTime;
            ExtraInfo = extraInfo;
            Message = message;
        }

        #endregion
    }
}