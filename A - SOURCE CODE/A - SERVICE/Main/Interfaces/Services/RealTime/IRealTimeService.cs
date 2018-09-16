using System.Threading;
using System.Threading.Tasks;
using Main.Models.RealTime;

namespace Main.Interfaces.Services.RealTime
{
    public interface IRealTimeService
    {
        #region Methods

        /// <summary>
        /// Add client to push group.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="groupName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task AddClientToPushGroupAsync(string clientId, string groupName, CancellationToken cancellationToken);

        /// <summary>
        /// Add client to real time group.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="userId"></param>
        /// <param name="groups"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task AddClientToRealTimeGroupsAsync(string clientId, int userId, string[] groups, CancellationToken cancellationToken);

            /// <summary>
        /// Send real-time notfication to specific groups.
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="eventName"></param>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SendToGroupsAsync<TMessageAdditionalData>(string[] groups, string eventName, RealTimeMessage<TMessageAdditionalData> message, CancellationToken cancellationToken);

        /// <summary>
        /// Send real-time notification to specific clients.
        /// </summary>
        /// <param name="clientIds"></param>
        /// <param name="eventName"></param>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SendToClientsAsync<TMessageAdditionalData>(string[] clientIds, string eventName, RealTimeMessage<TMessageAdditionalData> message, CancellationToken cancellationToken);

        #endregion
    }
}