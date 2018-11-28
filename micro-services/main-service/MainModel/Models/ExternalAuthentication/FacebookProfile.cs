using Newtonsoft.Json;

namespace MainModel.Models.ExternalAuthentication
{
    public class FacebookProfile
    {
        #region Properties

        /// <summary>
        ///     Facebook account id.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        ///     Facebook full name.
        /// </summary>
        [JsonProperty("name")]
        public string FullName { get; set; }

        /// <summary>
        ///     Email address which is used for facebook registration.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        #endregion
    }
}