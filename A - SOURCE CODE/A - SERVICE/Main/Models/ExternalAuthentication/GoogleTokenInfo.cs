using Newtonsoft.Json;

namespace Main.Models.ExternalAuthentication
{
    public class GoogleTokenInfo
    {
        #region Properties

        /// <summary>
        /// Access token from google api.
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Type of token.
        /// </summary>
        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        /// <summary>
        /// How many seconds that token lives.
        /// </summary>
        [JsonProperty("expires_in")]
        public int LifeTime { get; set; }

        /// <summary>
        /// Token id of google.
        /// </summary>
        [JsonProperty("id_token")]
        public string Id { get; set; }

        #endregion
    }
}