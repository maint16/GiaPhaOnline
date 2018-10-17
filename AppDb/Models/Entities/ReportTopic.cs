using AppShared.Enumerations;
using Newtonsoft.Json;

namespace AppDb.Models.Entities
{
    public class ReportTopic
    {
        #region Properties

        /// <summary>
        ///     Which topic is reported.
        /// </summary>
        public int TopicId { get; set; }

        /// <summary>
        ///     Who report the topic.
        /// </summary>
        public int ReporterId { get; set; }

        /// <summary>
        ///     Who owns the topic.
        /// </summary>
        public int OwnerId { get; set; }

        /// <summary>
        ///     Reason the topic was reported.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        ///     Status of topic report.
        /// </summary>
        public ItemStatus Status { get; set; }

        /// <summary>
        ///     When the report was created.
        /// </summary>
        public double CreatedTime { get; set; }

        /// <summary>
        ///     When the report was lastly modified.
        /// </summary>
        public double? LastModifiedTime { get; set; }

        #endregion

        #region Relationships

        /// <summary>
        ///     One report is about one topic, just one.
        /// </summary>
        [JsonIgnore]
        public Topic Topic { get; set; }

        /// <summary>
        ///     Report can only be about one account.
        /// </summary>
        [JsonIgnore]
        public User TopicOwner { get; set; }

        /// <summary>
        ///     Report can only belong to one account.
        /// </summary>
        [JsonIgnore]
        public User TopicReporter { get; set; }

        #endregion
    }
}