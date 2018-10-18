using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ClientShared.Enumerations;
using Newtonsoft.Json;

namespace AppDb.Models.Entities
{
    public class Topic
    {
        #region Properties

        /// <summary>
        ///     Id of topcic.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Who owns the post.
        /// </summary>
        public int OwnerId { get; set; }

        /// <summary>
        ///     Category that topic belongs to.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        ///     Category group that topic belongs to.
        /// </summary>
        public int CategoryGroupId { get; set; }

        /// <summary>
        ///     Title of topic.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Topic body.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        ///     Type of topic.
        /// </summary>
        public TopicType Type { get; set; }

        /// <summary>
        ///     Status of topic.
        /// </summary>
        public ItemStatus Status { get; set; }

        /// <summary>
        ///     When the topic was created.
        /// </summary>
        [Required]
        public double CreatedTime { get; set; }

        /// <summary>
        ///     When the topic was lastly modified.
        /// </summary>
        public double? LastModifiedTime { get; set; }

        #endregion

        #region Relationships

        /// <summary>
        ///     Who create the post.
        /// </summary>
        [JsonIgnore]
        public User Owner { get; set; }

        /// <summary>
        ///     Category which topic belongs to.
        /// </summary>
        [JsonIgnore]
        public Category Category { get; set; }

        /// <summary>
        ///     List of reply belongs to the post.
        /// </summary>
        [JsonIgnore]
        public ICollection<Reply> Replies { get; set; }

        /// <summary>
        ///     One topic can be monitored by follow topic.
        /// </summary>
        [JsonIgnore]
        public ICollection<FollowTopic> FollowTopics { get; set; }

        /// <summary>
        ///     One topic can have many reports about it.
        /// </summary>
        [JsonIgnore]
        public ICollection<ReportTopic> ReportTopics { get; set; }

        [JsonIgnore]
        public virtual CategorySummary CategorySummary { get; set; }

        [JsonIgnore]
        public virtual TopicSummary TopicSummary { get; set; }

        #endregion

        #region Constructors

        public Topic()
        {
        }

        public Topic(int id, int ownerId, int categoryId, int categoryGroupId, string title, string body,
            TopicType type, ItemStatus status, double createdTime, double? lastModifiedTime)
        {
            Id = id;
            OwnerId = ownerId;
            CategoryId = categoryId;
            CategoryGroupId = categoryGroupId;
            Title = title;
            Body = body;
            Type = type;
            Status = status;
            CreatedTime = createdTime;
            LastModifiedTime = lastModifiedTime;
        }

        #endregion
    }
}