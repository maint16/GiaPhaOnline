using System.Collections.Generic;

namespace Main.Interfaces.Services
{
    public interface ISendMailService
    {
        #region Methods

        /// <summary>
        /// Send email with specific information.
        /// </summary>
        /// <param name="recipients">List of recipient email addresses.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="content">Email content</param>
        void Send(List<string> recipients, string subject, string content);

        /// <summary>
        /// Send email with specific information.
        /// </summary>
        /// <param name="recipients"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        /// <param name="isHtmlContent"></param>
        void Send(List<string> recipients, string subject, string content, bool isHtmlContent);

        #endregion
    }
}