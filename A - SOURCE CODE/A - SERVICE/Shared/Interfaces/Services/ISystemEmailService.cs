using System.Threading.Tasks;
using Shared.Enumerations;

namespace Shared.Interfaces.Services
{
    public interface ISystemEmailService
    {
        /// <summary>
        /// Key which is used for api requests.
        /// </summary>
        string ApiKey { get; set; }
        
        /// <summary>
        /// Load email by template from a specific path.
        /// </summary>
        /// <param name="systemEmail"></param>
        /// <param name="absolutePath"></param>
        void LoadEmail(SystemEmail systemEmail, string absolutePath);

        /// <summary>
        /// Load email by searching related template.
        /// </summary>
        /// <param name="systemEmail"></param>
        /// <returns></returns>
        string LoadEmailContent(SystemEmail systemEmail);

        /// <summary>
        /// Send email to a list of recipients with subject and content.
        /// </summary>
        /// <param name="recipients"></param>
        /// <param name="subject"></param>
        /// <param name="html"></param>
        /// <returns></returns>
        void Send(string[] recipients, string subject, string html);
    }
}