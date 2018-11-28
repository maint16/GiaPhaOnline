using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MainMicroService.Interfaces.Services;
using MainMicroService.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace MainMicroService.Services
{
    public class SendGridService : ISendMailService
    {
        #region Constructor

        /// <summary>
        ///     Initialize service with injectors.
        /// </summary>
        /// <param name="sendGridSettingOptions">SendGrid configuration.</param>
        /// <param name="sendGridServiceLogger"></param>
        public SendGridService(IOptions<SendGridSetting> sendGridSettingOptions,
            ILogger<SendGridService> sendGridServiceLogger)
        {
            var sendGridSetting = sendGridSettingOptions.Value;
            _sendGridClient = new SendGridClient(sendGridSetting.ApiKey);
            _sendGridSetting = sendGridSettingOptions.Value;
            _sendGridServiceLogger = sendGridServiceLogger;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="recipients"></param>
        /// <param name="carbonCopies"></param>
        /// <param name="blindCarbonCopies"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        /// <param name="bIsHtmlContent"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SendAsync(HashSet<string> recipients, HashSet<string> carbonCopies,
            HashSet<string> blindCarbonCopies, string subject, string content, bool bIsHtmlContent,
            CancellationToken cancellationToken)
        {
            // No recipient has been found.
            if (recipients == null || recipients.Count < 1)
                throw new Exception("Recipients list is empty.");

            // Subject is empty.
            if (string.IsNullOrWhiteSpace(subject))
                throw new Exception("Subject is empty");

            // Content is empty.
            if (string.IsNullOrWhiteSpace(content))
                throw new Exception("Content is empty");

            // Initialize SendGridMessage.
            var sendGridMessage = new SendGridMessage();

            // Add recipient to list.
            if (recipients.Count > 0)
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

            // Debug log.
#if DEBUG
            _sendGridServiceLogger.LogDebug(
                $"Sent mail to {recipients.ToList()} with subject {subject} and content {content}");
#endif
            // Send mail asynchronously.
            await _sendGridClient.SendEmailAsync(sendGridMessage, cancellationToken);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Sendgrid client to make request to SendGrid service.
        /// </summary>
        private readonly SendGridClient _sendGridClient;

        /// <summary>
        ///     Settings of sendgrid service.
        /// </summary>
        private readonly SendGridSetting _sendGridSetting;

        /// <summary>
        ///     Sendgrid client logger.
        /// </summary>
        private readonly ILogger<SendGridService> _sendGridServiceLogger;

        #endregion
    }
}