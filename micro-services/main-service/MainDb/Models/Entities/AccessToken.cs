using Newtonsoft.Json;

namespace AppDb.Models.Entities
{
    public class AccessToken
    {
        #region Navigation properties

        /// <summary>
        ///     Who own the access token.
        /// </summary>
        [JsonIgnore]
        public virtual User Owner { get; set; }

        #endregion

        #region Properties

        public string Code { get; set; }

        public int OwnerId { get; set; }

        public double IssuedTime { get; set; }

        public double? ExpiredTime { get; set; }

        #endregion
    }
}