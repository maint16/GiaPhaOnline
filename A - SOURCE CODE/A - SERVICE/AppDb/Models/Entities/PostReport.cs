using System.ComponentModel.DataAnnotations.Schema;
using AppModel.Enumerations;
using Newtonsoft.Json;

namespace AppDb.Models.Entities
{
    public class PostReport
    {
        #region Properties

        /// <summary>
        ///     Which post is reported.
        /// </summary>
        public int PostId { get; set; }

        /// <summary>
        ///     Who owns the post.
        /// </summary>
        public int OwnerId { get; set; }

        /// <summary>
        ///     Who report the post.
        /// </summary>
        public int ReporterId { get; set; }

        /// <summary>
        ///     Original content of post.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        ///     Reason the post was reported.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Status of post report.
        /// </summary>
        public PostReportStatus Status { get; set; }

        /// <summary>
        ///     When the report was created.
        /// </summary>
        public double CreatedTime { get; set; }

        /// <summary>
        /// When the report was lastly modified.
        /// </summary>
        public double? LastModifiedTime { get; set; }

        #endregion

        #region Relationships

        /// <summary>
        ///     One report is about one post, just one.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(PostId))]
        public Post Post { get; set; }

        /// <summary>
        ///     Report can only be about one account.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(OwnerId))]
        public Account PostOwner { get; set; }

        /// <summary>
        ///     Report can only belong to one account.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(ReporterId))]
        public Account PostReporter { get; set; }

        #endregion
    }
}