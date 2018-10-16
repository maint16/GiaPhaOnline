namespace AuthenticationModel.Models.ExternalAuthentication
{
    public class FacebookCredential
    {
        #region Properties

        /// <summary>
        ///     Id of client.
        ///     Please refer: https://developers.facebook.com/docs/facebook-login/manually-build-a-login-flow
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        ///     Secret key
        ///     Please refer: https://developers.facebook.com/docs/facebook-login/manually-build-a-login-flow
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        ///     Uri which is for redirection. Must be the same as client.
        /// </summary>
        public string RedirectUri { get; set; }

        #endregion
    }
}