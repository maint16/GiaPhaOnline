namespace AuthenticationMicroService.Constants
{
    public class FacebookUrlConstant
    {
        #region Properties

        /// <summary>
        ///     Url to get facebook token url.
        /// </summary>
        public const string GetFacebookTokenInfoUrl = "https://graph.facebook.com/v2.12/oauth/access_token";

        /// <summary>
        ///     Url to get facebook profile.
        /// </summary>
        public const string GetFacebookProfileUrl = "https://graph.facebook.com/v2.12/me";

        #endregion
    }
}