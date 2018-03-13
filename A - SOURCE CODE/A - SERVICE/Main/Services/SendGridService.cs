using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Main.Interfaces.Services;
using Main.Models;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Main.Services
{
    public class SendGridService : ISendMailService
    {
        #region Properties

        /// <summary>
        /// Sendgrid client to make request to SendGrid service.
        /// </summary>
        private readonly SendGridClient _sendGridClient;

        /// <summary>
        /// Settings of sendgrid service.
        /// </summary>
        private readonly SendGridSetting _sendGridSetting;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize service with injectors.
        /// </summary>
        /// <param name="sendGridSettingOptions">SendGrid configuration.</param>
        public SendGridService(IOptions<SendGridSetting> sendGridSettingOptions)
        {
            var sendGridSetting = sendGridSettingOptions.Value;
            _sendGridClient = new SendGridClient(sendGridSetting.ApiKey);
            _sendGridSetting = sendGridSettingOptions.Value;
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public async Task SendAsync(HashSet<string> recipients, HashSet<string> carbonCopies, HashSet<string> blindCarbonCopies, string subject, string content, bool bIsHtmlContent, CancellationToken cancellationToken)
        {
            // Initialize SendGridMessage.
            var sendGridMessage = new SendGridMessage();

            // Add recipient to list.
            if (recipients != null && recipients.Count > 0)
                sendGridMessage.AddTos(recipients.Select(x => new EmailAddress(x)).ToList());

            // Add carbon copy.
            if (carbonCopies != null && carbonCopies.Count > 0)
                sendGridMessage.AddCcs(carbonCopies.Select(x => new EmailAddress(x)).ToList());

            // Add blind carbon copy.
            if (blindCarbonCopies != null && blindCarbonCopies.Count > 0)
                sendGridMessage.AddBccs(blindCarbonCopies.Select(x => new EmailAddress(x)).ToList());

            sendGridMessage.From = _sendGridSetting.From;
            sendGridMessage.Subject = subject;

            if (bIsHtmlContent)
                sendGridMessage.HtmlContent = content;
            else
                sendGridMessage.PlainTextContent = content;

            // Send mail asynchronously.
            await _sendGridClient.SendEmailAsync(sendGridMessage, cancellationToken);
        }
        
        #endregion
    }
}