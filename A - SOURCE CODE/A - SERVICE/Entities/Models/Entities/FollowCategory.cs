using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SystemDatabase.Models.Entities
{
    public class FollowCategory
    {
        #region Properties

        /// <summary>
        ///     Owner of following relationship.
        /// </summary>
        public int OwnerId { get; set; }

        /// <summary>
        ///     Category index.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        ///     When the relationship was lastly created.
        /// </summary>
        public double CreatedTime { get; set; }

        /// <summary>
        /// When the category was lastly modified.
        /// </summary>
        public double? LastModifiedTime { get; set; }

        #endregion

        #region Relationships

        /// <summary>
        ///     Who starts watching.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(OwnerId))]
        public virtual Account Owner { get; set; }

        /// <summary>
        ///     Which is being watched.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; }

        #endregion
    }
}