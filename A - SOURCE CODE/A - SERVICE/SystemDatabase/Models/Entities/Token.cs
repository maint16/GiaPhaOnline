using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SystemDatabase.Enumerations;
using Newtonsoft.Json;

namespace SystemDatabase.Models.Entities
{
    public class Token
    {
        #region Relationships

        /// <summary>
        ///     One category have one owner.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(OwnerId))]
        public Account Owner { get; set; }

        #endregion

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
        public int OwnerId { get; set; }

        /// <summary>
        ///     Type of Token.
        /// </summary>
        public TokenType Type { get; set; }

        /// <summary>
        ///     Code of token
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        ///     Time when the token was issued.
        /// </summary>
        public double IssuedTime { get; set; }

        /// <summary>
        ///     Time when the token should be expired.
        /// </summary>
        public double? ExpiredTime { get; set; }

        #endregion
    }
}