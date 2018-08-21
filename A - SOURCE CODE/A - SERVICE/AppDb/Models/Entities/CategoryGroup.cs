using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace AppDb.Models.Entities
{
    public class CategoryGroup
    {
        #region Properties

        /// <summary>
        ///     Id of category group.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        ///     Who created the current category group.
        /// </summary>
        [Required]
        public int CreatorId { get; set; }

        /// <summary>
        ///     Name of category group.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        ///     Description of category group.
        /// </summary>
        [Required]
        public string Description { get; set; }

        /// <summary>
        ///     When the category group was created
        /// </summary>
        [Required]
        public double CreatedTime { get; set; }

        /// <summary>
        ///     When the category group was lastly modified.
        /// </summary>
        public double? LastModifiedTime { get; set; }

        #endregion

        #region Relationships

        /// <summary>
        ///     One category can only be created by one account.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(CreatorId))]
        public Account Creator { get; set; }

        /// <summary>
        ///     List of category which are related to the current category group.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<Category> Categories { get; set; }

        #endregion
    }
}
