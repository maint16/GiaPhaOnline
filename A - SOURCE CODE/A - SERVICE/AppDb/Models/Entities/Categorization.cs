using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace AppDb.Models.Entities
{
    public class Categorization
    {
        #region Properties

        /// <summary>
        /// Id of category.
        /// </summary>
        public int CategoryId { get; set; }
        
        /// <summary>
        /// Post id
        /// </summary>
        public int PostId { get; set; }

        /// <summary>
        /// Categorization time.
        /// </summary>
        public double CategorizationTime { get; set; }

        #endregion

        #region Relationships

        /// <summary>
        /// Category which has been categorized.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }

        /// <summary>
        /// Post which has been categorized.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(PostId))]
        public Post Post { get; set; }

        #endregion
    }
}