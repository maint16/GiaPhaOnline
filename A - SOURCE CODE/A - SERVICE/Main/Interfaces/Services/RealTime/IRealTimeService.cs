using System.Threading;
using System.Threading.Tasks;
using Main.Models.RealTime;

namespace Main.Interfaces.Services.RealTime
{
    public interface IRealTimeService
    {
        #region Methods
        
        /// <summary>
        /// Send real-time notification to specific clients.
        /// </summary>
        /// <param name="clientIds"></param>
        /// <param name="eventName"></param>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SendToClientsAsync<T>(string[] clientIds, string eventName, T message, CancellationToken cancellationToken);

        /// <summary>
        /// Send real-time notification to specific group.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="groups"></param>
        /// <param name="eventName"></param>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SendToGroupsAsync<T>(string[] groups, string eventName,
            T message, CancellationToken cancellationToken);

        #endregion
    }
}