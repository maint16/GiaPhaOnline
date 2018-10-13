using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Main.Interfaces.Services
{
    public interface ISendMailService
    {
        #region Methods

        /// <summary>
        ///     Send email with specific information.
        /// </summary>
        /// <param name="recipients">List of recipient email addresses.</param>
        /// <param name="carbonCopies"></param>
        /// <param name="blindCarbonCopies"></param>
        /// <param name="subject">Email subject.</param>
        /// <param name="content">Email content</param>
        /// <param name="bIsHtmlContent"></param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task SendAsync(HashSet<string> recipients, HashSet<string> carbonCopies, HashSet<string> blindCarbonCopies,
            string subject, string content, bool bIsHtmlContent, CancellationToken cancellationToken);

        #endregion
    }
}