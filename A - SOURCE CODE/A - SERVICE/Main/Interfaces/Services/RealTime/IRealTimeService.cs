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
        Task SendToClientsAsync<TMessageAdditionalData>(string[] clientIds, string eventName, RealTimeMessage<TMessageAdditionalData> message, CancellationToken cancellationToken);

        /// <summary>
        /// Send real-time notification to specific group.
        /// </summary>
        /// <typeparam name="TMessageAdditionalData"></typeparam>
        /// <param name="groups"></param>
        /// <param name="eventName"></param>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SendToGroupsAsync<TMessageAdditionalData>(string[] groups, string eventName,
            RealTimeMessage<TMessageAdditionalData> message, CancellationToken cancellationToken);

        #endregion
    }
}