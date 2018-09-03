using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace AppDb.Models.Entities
{
    public class AccessToken
    {
        #region Properties

        /// <summary>
        ///     Id of token.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        ///     Who this token belongs to.
        /// </summary>
        [Required]
        public int OwnerId { get; set; }

        /// <summary>
        ///     Code of token
        /// </summary>
        [Required]
        public string Code { get; set; }

        /// <summary>
        ///     Time when the token was issued.
        /// </summary>
        [Required]
        public double IssuedTime { get; set; }

        /// <summary>
        ///     Time when the token should be expired.
        /// </summary>
        public double? ExpiredTime { get; set; }

        #endregion

        #region Relationships

        /// <summary>
        ///     One category have one owner.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(OwnerId))]
        public Account Owner { get; set; }

        #endregion
    }
}