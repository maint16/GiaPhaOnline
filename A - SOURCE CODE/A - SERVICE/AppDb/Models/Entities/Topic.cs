using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AppModel.Enumerations;
using Newtonsoft.Json;

namespace AppDb.Models.Entities
{
    public class Topic
    {
        #region Properties

        /// <summary>
        ///     Id of topcic.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        ///     Who owns the post.
        /// </summary>
        [Required]
        public int OwnerId { get; set; }

        /// <summary>
        ///     Category that topic belongs to.
        /// </summary>
        [Required]
        public int CategoryId { get; set; }

        /// <summary>
        ///     Title of topic.
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        ///     Topic body.
        /// </summary>
        [Required]
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
        [ForeignKey(nameof(OwnerId))]
        public Account Owner { get; set; }

        /// <summary>
        ///     Category which topic belongs to.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(CategoryId))]
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

        #endregion
    }
}