using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using AppModel.Enumerations;
using Newtonsoft.Json;

namespace AppDb.Models.Entities
{
    public class NotificationMessage
    {
        #region Properties

        /// <summary>
        ///     Id of notification
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        ///     Owner of notification
        /// </summary>
        [Required]
        public int OwnerId { get; set; }

        /// <summary>
        ///     Type of notification
        /// </summary>
        public NotificationKinds Type { get; set; }

        /// <summary>
        ///     Status of notification
        /// </summary>
        public NotificationStatus Status { get; set; }

        /// <summary>
        ///     When the notification message was created.
        /// </summary>
        [Required]
        public double CreatedTime { get; set; }

        /// <summary>
        ///     Information of notification
        /// </summary>
        public string ExtraInfo { get; set; }

        /// <summary>
        ///     Message of notification
        /// </summary>
        [Required]
        public string Message { get; set; }

        #endregion

        #region Relationships

        /// <summary>
        ///     One notification can only be initiated by one account.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(OwnerId))]
        public User Owner { get; set; }

        #endregion
    }
}
