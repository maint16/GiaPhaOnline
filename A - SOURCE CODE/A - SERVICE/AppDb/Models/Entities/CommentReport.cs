using System.ComponentModel.DataAnnotations.Schema;
using AppModel.Enumerations;
using Newtonsoft.Json;

namespace AppDb.Models.Entities
{
    public class CommentReport
    {
        #region Properties

        /// <summary>
        ///     Comment which is reported.
        /// </summary>
        public int CommentId { get; set; }

        /// <summary>
        ///     The post comment belongs to.
        /// </summary>
        public int PostId { get; set; }

        /// <summary>
        ///     Owner of comment.
        /// </summary>
        public int OwnerId { get; set; }

        /// <summary>
        ///     Who created the report.
        /// </summary>
        public int ReporterId { get; set; }

        /// <summary>
        ///     Content of original comment.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        ///     Reason the comment is reported.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Status of comment report.
        /// </summary>
        public ItemStatus Status { get; set; }

        /// <summary>
        ///     When the report was created.
        /// </summary>
        public double CreatedTime { get; set; }

        /// <summary>
        /// Time when comment report has been modified.
        /// </summary>
        public double? LastModifiedTime { get; set; }

        #endregion

        #region Relationships

        /// <summary>
        ///     One report can only belongs to one comment.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(CommentId))]
        public Comment Comment { get; set; }

        /// <summary>
        ///     One report can only belongs to one post.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(PostId))]
        public Post Post { get; set; }

        /// <summary>
        ///     Account which owns the comment
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(OwnerId))]
        public Account CommentOwner { get; set; }

        /// <summary>
        ///     One report belongs to one specific account.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(ReporterId))]
        public Account CommentReporter { get; set; }

        #endregion
    }
}