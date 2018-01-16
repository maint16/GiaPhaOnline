using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SystemDatabase.Enumerations;
using Newtonsoft.Json;

namespace SystemDatabase.Models.Entities
{
    public class CommentNotification
    {
        #region Properties

        /// <summary>
        ///     Id of notification.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        ///     Comment which causes notification broadcasted.
        /// </summary>
        public int CommentId { get; set; }

        /// <summary>
        ///     Post which comment belongs to.
        /// </summary>
        public int PostId { get; set; }

        /// <summary>
        ///     Owner of notification.
        /// </summary>
        public int RecipientId { get; set; }

        /// <summary>
        ///     Who causes the notification broadcasted.
        /// </summary>
        public int BroadcasterId { get; set; }

        /// <summary>
        ///     Notification type.
        /// </summary>
        public NotificationType Type { get; set; }

        /// <summary>
        ///     Whether notification owner has seen the notification or not.
        /// </summary>
        public bool IsSeen { get; set; }

        /// <summary>
        ///     When the notification was created.
        /// </summary>
        public double Created { get; set; }

        #endregion

        #region Relationships

        /// <summary>
        ///     Who should receive the notification.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(RecipientId))]
        public Account Recipient { get; set; }

        /// <summary>
        ///     Who broadcasted the notification.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(BroadcasterId))]
        public Account Broadcaster { get; set; }

        /// <summary>
        ///     Comment which is notified.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(CommentId))]
        public Comment Comment { get; set; }

        /// <summary>
        ///     Which post comment belongs to.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(PostId))]
        public Post Post { get; set; }

        #endregion
    }
}