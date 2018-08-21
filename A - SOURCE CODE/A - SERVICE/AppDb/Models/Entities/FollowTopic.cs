using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AppModel.Enumerations;
using Newtonsoft.Json;

namespace AppDb.Models.Entities
{
    public class FollowTopic
    {
        #region Properties

        /// <summary>
        ///     Who is the follower of post.
        /// </summary>
        [Key]
        public int FollowerId { get; set; }

        /// <summary>
        ///     Which topci is being followed by the follower.
        /// </summary>
        [Key]
        public int TopicId { get; set; }

        /// <summary>
        /// Status of follow post.
        /// </summary>
        public ItemStatus Status { get; set; }

        /// <summary>
        ///     When the following action was created.
        /// </summary>
        [Required]
        public double CreatedTime { get; set; }

        #endregion

        #region Relationships

        /// <summary>
        ///     Who is following the post.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(FollowerId))]
        public Account Follower { get; set; }

        /// <summary>
        ///     Topic which is being monitored by this relationship.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(TopicId))]
        public Topic Topic { get; set; }

        #endregion
    }
}