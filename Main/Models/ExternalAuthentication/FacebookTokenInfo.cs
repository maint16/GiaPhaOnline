using Newtonsoft.Json;

namespace Main.Models.ExternalAuthentication
{
    public class FacebookTokenInfo
    {
        #region Properties

        /// <summary>
        ///     Access token.
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        ///     Type of token.
        /// </summary>
        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        /// <summary>
        ///     Token life time.
        /// </summary>
        [JsonProperty("expires_in")]
        public int LifeTime { get; set; }

        #endregion
    }
}