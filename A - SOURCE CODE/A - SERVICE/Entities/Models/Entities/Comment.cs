using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SystemDatabase.Models.Entities
{
    public class Comment
    {
        #region Properties

        /// <summary>
        ///     Id of comment (Auto incremented)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        ///     Who wrote the comment.
        /// </summary>
        public int OwnerId { get; set; }

        /// <summary>
        ///     Which post this comment belongs to.
        /// </summary>
        public int PostId { get; set; }

        /// <summary>
        ///     Comment content.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        ///     When was the comment created.
        /// </summary>
        public double Created { get; set; }

        /// <summary>
        ///     When the comment was lastly modified.
        /// </summary>
        public double? LastModified { get; set; }

        #endregion

        #region Relationships

        /// <summary>
        ///     One comment can only be initiated by one account.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(OwnerId))]
        public Account Owner { get; set; }

        /// <summary>
        ///     One comment can only belong to one post.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(PostId))]
        public Post Post { get; set; }

        #endregion
    }
}