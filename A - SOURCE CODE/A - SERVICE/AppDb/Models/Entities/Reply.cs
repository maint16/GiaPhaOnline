using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AppModel.Enumerations;
using Newtonsoft.Json;

namespace AppDb.Models.Entities
{
    public class Reply
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
        [Required]
        public int OwnerId { get; set; }

        /// <summary>
        ///     Which post this comment belongs to.
        /// </summary>
        [Required]
        public int TopicId { get; set; }

        /// <summary>
        ///     Comment content.
        /// </summary>
        [Required]
        public string Content { get; set; }

        /// <summary>
        /// Status of comment.
        /// </summary>
        public ItemStatus Status { get; set; }

        /// <summary>
        ///     When was the comment created.
        /// </summary>
        [Required]
        public double CreatedTime { get; set; }

        /// <summary>
        ///     When the comment was lastly modified.
        /// </summary>
        public double? LastModifiedTime { get; set; }

        #endregion

        #region Relationships

        /// <summary>
        ///     One comment can only be initiated by one account.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(OwnerId))]
        public Account Owner { get; set; }

        /// <summary>
        ///     One reply can only belong to one topic.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(TopicId))]
        public Topic Topic { get; set; }

        #endregion
    }
}