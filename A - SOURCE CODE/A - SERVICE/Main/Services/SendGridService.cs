using System.Collections.Generic;
using Main.Interfaces;
using Main.Interfaces.Services;

namespace Main.Services
{
    public class SendGridService : ISendMailService
    {
        #region Methods
        
        /// <inheritdoc/>
        /// <param name="recipients"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        public void Send(List<string> recipients, string subject, string content)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        /// <param name="recipients"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        /// <param name="isHtmlContent"></param>
        public void Send(List<string> recipients, string subject, string content, bool isHtmlContent)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}