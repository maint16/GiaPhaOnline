using System.Collections.Generic;
using System.Threading.Tasks;
using SystemConstant.Enumerations;
using Microsoft.AspNetCore.SignalR;

namespace Main.Interfaces
{
    public interface INotifyService
    {
        #region Methods

        /// <summary>
        /// Find clients with specific roles and send push notification & real-time notification to.
        /// </summary>
        /// <param name="hubContext"></param>
        /// <param name="roles"></param>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task[] NotifyClients(IHubContext<Hub> hubContext, AccountRole[] roles, string title, string message, Dictionary<string, string> data);

        #endregion
    }
}