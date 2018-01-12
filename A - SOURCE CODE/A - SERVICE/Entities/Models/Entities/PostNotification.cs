using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SystemDatabase.Models.Entities
{
    public class PostNotification
    {
        #region Properties

        /// <summary>
        ///     Id of notification.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        ///     Post which is notified.
        /// </summary>
        public int PostIndex { get; set; }

        /// <summary>
        ///     Who should receive the notification.
        /// </summary>
        public int RecipientIndex { get; set; }

        /// <summary>
        ///     Who caused the notification broadcasted.
        /// </summary>
        public int BroadcasterIndex { get; set; }

        /// <summary>
        ///     Type of notification (CRUD)
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        ///     Whether the owner seen the post or not.
        /// </summary>
        public bool IsSeen { get; set; }

        /// <summary>
        ///     When the notification was created.
        /// </summary>
        public double Created { get; set; }

        #endregion

        #region Relationships

        /// <summary>
        ///     Post which is notified.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(PostIndex))]
        public Post Post { get; set; }

        /// <summary>
        ///     Who broadcasted the notification.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(RecipientIndex))]
        public Account Recipient { get; set; }

        /// <summary>
        ///     Who should receive the notification.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(BroadcasterIndex))]
        public Account Broadcaster { get; set; }

        #endregion
    }
}