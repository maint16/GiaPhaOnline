using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AppModel.Enumerations;
using Newtonsoft.Json;

namespace AppDb.Models.Entities
{
    public class PostNotification
    {
        #region Properties

        /// <summary>
        ///     Id of notification.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        ///     Post which is notified.
        /// </summary>
        public int PostId { get; set; }

        /// <summary>
        ///     Who should receive the notification.
        /// </summary>
        public int RecipientId { get; set; }

        /// <summary>
        ///     Who caused the notification broadcasted.
        /// </summary>
        public int BroadcasterId { get; set; }

        /// <summary>
        ///     Type of notification (CRUD)
        /// </summary>
        public NotificationType Type { get; set; }

        /// <summary>
        ///     Whether the owner seen the post or not.
        /// </summary>
        public NotificationStatus Status { get; set; }

        /// <summary>
        ///     When the notification was created.
        /// </summary>
        public double CreatedTime { get; set; }

        #endregion

        #region Relationships

        /// <summary>
        ///     Post which is notified.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(PostId))]
        public Topic Post { get; set; }

        /// <summary>
        ///     Who broadcasted the notification.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(RecipientId))]
        public Account Recipient { get; set; }

        /// <summary>
        ///     Who should receive the notification.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(BroadcasterId))]
        public Account Broadcaster { get; set; }

        #endregion
    }
}