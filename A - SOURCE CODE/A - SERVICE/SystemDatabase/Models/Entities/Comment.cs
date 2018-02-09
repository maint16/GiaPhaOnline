using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using SystemConstant.Enumerations;
using Newtonsoft.Json;

namespace SystemDatabase.Models.Entities
{
    public class Comment
    {
        #region Properties

        /// <summary>
        ///     Id of comment (Auto incremented)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Who wrote the comment.
        /// </summary>
        public int OwnerId { get; set; }

        /// <summary>
        ///     Which post this comment belongs to.
        /// </summary>
        public int PostId { get; set; }

        /// <summary>
        ///     Comment content.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Status of comment.
        /// </summary>
        public ItemStatus Status { get; set; }

        /// <summary>
        ///     When was the comment created.
        /// </summary>
        public double CreatedTime { get; set; }

        /// <summary>
        ///     When the comment was lastly modified.
        /// </summary>
        public double? LastModifiedTime { get; set; }

        #endregion

        #region Relationships

        /// <summary>
        ///     One comment can only be initiated by one account.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(OwnerId))]
        public Account Owner { get; set; }

        /// <summary>
        ///     One comment can only belong to one post.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(PostId))]
        public Post Post { get; set; }

        /// <summary>
        ///     List of notifications belong to this comment.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<CommentNotification> CommentNotifications { get; set; }

        /// <summary>
        ///     List of reports belong to this current comment.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<CommentReport> CommentReports { get; set; }

        #endregion
    }
}