using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AppModel.Enumerations;
using Newtonsoft.Json;

namespace AppDb.Models.Entities
{
    public class ReportTopic
    {
        #region Properties

        /// <summary>
        ///     Which topic is reported.
        /// </summary>
        [Key]
        public int TopicId { get; set; }

        /// <summary>
        ///     Who report the topic.
        /// </summary>
        [Key]
        public int ReporterId { get; set; }

        /// <summary>
        ///     Who owns the topic.
        /// </summary>
        [Required]
        public int OwnerId { get; set; }

        /// <summary>
        ///     Reason the topic was reported.
        /// </summary>
        [Required]
        public string Reason { get; set; }

        /// <summary>
        /// Status of topic report.
        /// </summary>
        public ItemStatus Status { get; set; }

        /// <summary>
        ///     When the report was created.
        /// </summary>
        [Required]
        public double CreatedTime { get; set; }

        /// <summary>
        /// When the report was lastly modified.
        /// </summary>
        public double? LastModifiedTime { get; set; }

        #endregion

        #region Relationships

        /// <summary>
        ///     One report is about one topic, just one.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(TopicId))]
        public Topic Topic { get; set; }

        /// <summary>
        ///     Report can only be about one account.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(OwnerId))]
        public Account TopicOwner { get; set; }

        /// <summary>
        ///     Report can only belong to one account.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(ReporterId))]
        public Account TopicReporter { get; set; }

        #endregion
    }
}