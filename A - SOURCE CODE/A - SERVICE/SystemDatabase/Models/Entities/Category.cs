using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using SystemConstant.Enumerations;
using Newtonsoft.Json;

namespace SystemDatabase.Models.Entities
{
    public class Category
    {
        #region Relationships

        /// <summary>
        ///     One category can only be created by one account.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(CreatorId))]
        public Account Creator { get; set; }

        /// <summary>
        ///     List of categorization which are related to the current category.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<Categorization> Categorizations { get; set; }

        /// <summary>
        ///     Category follow.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<FollowCategory> FollowCategories { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Id of category.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Who created the current category.
        /// </summary>
        public int CreatorId { get; set; }

        /// <summary>
        /// Photo relative url.
        /// </summary>
        [JsonProperty("Photo")]
        public string PhotoRelativeUrl { get; set; }

        /// <summary>
        /// Absolute url shouldn't be passed to client.
        /// </summary>
        [JsonIgnore]
        public string PhotoAbsoluteUrl { get; set; }

        /// <summary>
        ///     Status of category.
        /// </summary>
        public ItemStatus Status { get; set; }

        /// <summary>
        ///     Name of category.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     When the category was created.
        /// </summary>
        public double CreatedTime { get; set; }

        /// <summary>
        ///     When the category was lastly modified.
        /// </summary>
        public double? LastModifiedTime { get; set; }

        #endregion
    }
}