namespace Main.Models.ExternalAuthentication
{
    public class GoogleCredential
    {
        #region Properties

        /// <summary>
        /// Client id which has been provided by google developer console.
        /// Please refer : https://console.developers.google.com/apis/credentials
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Client secret which has been provided by google developer console.
        /// Please refer: https://console.developers.google.com/apis/credentials
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Redirect uri which must be the same as the defined one in google developer console.
        /// </summary>
        public string RedirectUri { get; set; }

        #endregion
    }
}