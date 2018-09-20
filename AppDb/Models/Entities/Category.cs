using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AppModel.Enumerations;
using Newtonsoft.Json;

namespace AppDb.Models.Entities
{
    public class Category
    {
        #region Properties

        /// <summary>
        ///     Id of category.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        ///     Who created the current category.
        /// </summary>
        [Required]
        public int CreatorId { get; set; }

        /// <summary>
        ///     Category group that category belongs to.
        /// </summary>
        [Required]
        public int CategoryGroupId { get; set; }

        /// <summary>
        ///     Photo of category
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        ///     Description of category
        /// </summary>
        [Required]
        public string Description { get; set; }

        /// <summary>
        ///     Status of category.
        /// </summary>
        public ItemStatus Status { get; set; }

        /// <summary>
        ///     Name of category.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        ///     When the category was created.
        /// </summary>
        [Required]
        public double CreatedTime { get; set; }

        /// <summary>
        ///     When the category was lastly modified.
        /// </summary>
        public double? LastModifiedTime { get; set; }

        #endregion

        #region Relationships

        /// <summary>
        ///     One category can only be created by one account.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(CreatorId))]
        public User Creator { get; set; }

        /// <summary>
        ///     Category group which category belongs to.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(CategoryGroupId))]
        public CategoryGroup CategoryGroup { get; set; }
        
        /// <summary>
        ///     Category follow.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<FollowCategory> FollowCategories { get; set; }

        /// <summary>
        ///     List of topic which are related to the current category.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<Topic> Topics { get; set; }

        #endregion
    }
}