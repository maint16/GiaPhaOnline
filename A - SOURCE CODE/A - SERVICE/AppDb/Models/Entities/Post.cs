using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AppModel.Enumerations;
using Newtonsoft.Json;

namespace AppDb.Models.Entities
{
    public class Post
    {
        #region Properties

        /// <summary>
        ///     Id of post.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        ///     Who owns the post.
        /// </summary>
        public int OwnerId { get; set; }

        /// <summary>
        ///     Title of post.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Post body.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        ///     Type of post.
        /// </summary>
        public PostType Type { get; set; }

        /// <summary>
        ///     Status of post.
        /// </summary>
        public PostStatus Status { get; set; }

        /// <summary>
        ///     When the post was created.
        /// </summary>
        public double CreatedTime { get; set; }

        /// <summary>
        ///     When the post was lastly modified.
        /// </summary>
        public double? LastModifiedTime { get; set; }

        #endregion

        #region Relationships

        /// <summary>
        ///     Who create the post.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(OwnerId))]
        public Account Owner { get; set; }

        /// <summary>
        ///     Category which post belongs to.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<Categorization> Categorizations { get; set; }

        /// <summary>
        ///     List of comment belongs to the post.
        /// </summary>
        [JsonIgnore]
        public ICollection<Comment> Comments { get; set; }

        /// <summary>
        ///     One post can be monitored by follow post.
        /// </summary>
        [JsonIgnore]
        public ICollection<FollowPost> FollowPosts { get; set; }

        /// <summary>
        ///     Which notification comment post belongs to.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<CommentNotification> CommentNotifications { get; set; }

        /// <summary>
        ///     Which notification post the post belongs to.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<PostNotification> PostNotifications { get; set; }

        /// <summary>
        ///     One post can have many reports about its comments.
        /// </summary>
        [JsonIgnore]
        public ICollection<CommentReport> ReportedComments { get; set; }

        /// <summary>
        ///     One post can have many reports about it.
        /// </summary>
        [JsonIgnore]
        public ICollection<PostReport> PostReports { get; set; }

        #endregion
    }
}