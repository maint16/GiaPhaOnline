using System.ComponentModel.DataAnnotations.Schema;
using AppModel.Enumerations;
using Newtonsoft.Json;

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
        /// Status of follow category.
        /// </summary>
        public ItemStatus Status { get; set; }

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
        [ForeignKey(nameof(FollowerId))]
        public virtual Account Follower { get; set; }

        /// <summary>
        ///     Which is being watched.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; }

        #endregion
    }
}