using System.ComponentModel.DataAnnotations.Schema;
using SystemConstant.Enumerations;
using Newtonsoft.Json;

namespace SystemDatabase.Models.Entities
{
    public class FollowPost
    {
        #region Properties

        /// <summary>
        ///     Who is the follower of post.
        /// </summary>
        public int FollowerId { get; set; }

        /// <summary>
        ///     Which post is being followed by the follower.
        /// </summary>
        public int PostId { get; set; }

        /// <summary>
        /// Status of follow post.
        /// </summary>
        public ItemStatus Status { get; set; }

        /// <summary>
        ///     When the following action was created.
        /// </summary>
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
        ///     Post which is being monitored by this relationship.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(PostId))]
        public Post Post { get; set; }

        #endregion
    }
}