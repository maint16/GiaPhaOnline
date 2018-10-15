using Newtonsoft.Json;
using Shared.Enumerations;

namespace AppDb.Models.Entities
{
    public class FollowCategory
    {
        #region Properties

        /// <summary>
        ///     Owner of following relationship.
        /// </summary>
        public int FollowerId { get; set; }

        /// <summary>
        ///     Category index.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        ///     Status of follow category.
        /// </summary>
        public FollowStatus Status { get; set; }

        /// <summary>
        ///     When the relationship was lastly created.
        /// </summary>
        public double CreatedTime { get; set; }

        #endregion

        #region Relationships

        /// <summary>
        ///     Who starts watching.
        /// </summary>
        [JsonIgnore]
        public virtual User Follower { get; set; }

        /// <summary>
        ///     Which is being watched.
        /// </summary>
        [JsonIgnore]
        public virtual Category Category { get; set; }

        #endregion
    }
}