namespace AuthenticationMicroService.Models.Captcha
{
    public class CaptchaSetting
    {
        #region Properties

        /// <summary>
        /// Google captcha secret.
        /// </summary>
        public string GoogleCaptchaSecret { get; set; }

        /// <summary>
        /// Google captcha remote end-point.
        /// </summary>
        public string GoogleCaptchaValidationEndpoint { get; set; }

        #endregion
    }
}
