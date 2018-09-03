using System.Collections.Generic;
using System.Threading.Tasks;
using Main.Models;
using Microsoft.AspNetCore.SignalR;

namespace Main.Interfaces.Services
{
    public interface IRealTimeNotificationService
    {
        /// <summary>
        /// Broadcast notification to specific clients.
        /// </summary>
        /// <param name="clientProxy"></param>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        Task BroadcastAsync(IClientProxy clientProxy, string methodName, object parameters = null);

        /// <summary>
        /// Broadcast notification to specific connection ids.
        /// </summary>
        /// <param name="hubContext"></param>
        /// <param name="connectionIds"></param>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task BroadcastAsync<T>(IHubContext<T> hubContext, IReadOnlyList<string> connectionIds, string methodName, object parameters = null) where T : Hub;
    }
}