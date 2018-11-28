using SendGrid.Helpers.Mail;

namespace Main.Models
{
    public class SendGridSetting
    {
        #region Properties

        /// <summary>
        ///     Api key.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        ///     Email address which mail is sent from.
        /// </summary>
        public EmailAddress From { get; set; }

        #endregion
    }
}