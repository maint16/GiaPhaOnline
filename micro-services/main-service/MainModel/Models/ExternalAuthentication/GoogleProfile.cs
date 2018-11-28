using Newtonsoft.Json;

namespace MainModel.Models.ExternalAuthentication
{
    public class GoogleProfile
    {
        #region Properties

        /// <summary>
        ///     Google display name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Account picture.
        /// </summary>
        public string Picture { get; set; }

        /// <summary>
        ///     Given name.
        /// </summary>
        [JsonProperty("given_name")]
        public string GivenName { get; set; }

        /// <summary>
        ///     Family name.
        /// </summary>
        [JsonProperty("family_name")]
        public string FamilyName { get; set; }

        /// <summary>
        ///     Local of user.
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        ///     Google email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     Whether email is verified or not.
        /// </summary>
        [JsonProperty("email_verified")]
        public bool IsEmailVerified { get; set; }

        #endregion
    }
}