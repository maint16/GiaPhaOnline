using System.Collections.Generic;
using System.Threading.Tasks;
using SystemConstant.Enumerations;
using SystemDatabase.Models.Entities;
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
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task[] NotifyClients<THub>(IHubContext<THub> hubContext, AccountRole[] roles, string title, string message,
            string eventName, Dictionary<string, object> data) where THub : Hub;

        #endregion
    }
}