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
    }
}