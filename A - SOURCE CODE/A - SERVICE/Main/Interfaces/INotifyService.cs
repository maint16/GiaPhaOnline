using System.Collections;
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

        /// <summary>
        /// Send notification to clients who are in pre-defined groups.
        /// </summary>
        /// <typeparam name="THub"></typeparam>
        /// <param name="hubContext"></param>
        /// <param name="groups"></param>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task[] NotifyClients<THub>(IHubContext<THub> hubContext, IReadOnlyList<string> groups, string title, string message,
            string eventName, IDictionary data) where THub : Hub;

        #endregion
    }
}