﻿using System.Threading;
using System.Threading.Tasks;
using MainDb.Models.Entities;
using MainMicroService.Models.RealTime;

namespace MainMicroService.Interfaces.Services.RealTime
{
    public interface IRealTimeService
    {
        #region Methods

        /// <summary>
        ///     Send real-time notification to specific clients.
        /// </summary>
        /// <param name="clientIds"></param>
        /// <param name="eventName"></param>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SendRealTimeMessageToClientsAsync<T>(string[] clientIds, string eventName, T message,
            CancellationToken cancellationToken);

        /// <summary>
        ///     Send real-time notification to specific group.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="groups"></param>
        /// <param name="eventName"></param>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SendRealTimeMessageToGroupsAsync<T>(string[] groups, string eventName,
            T message, CancellationToken cancellationToken);

        /// <summary>
        ///     Send push message to specific groups of users.
        /// </summary>
        /// <returns></returns>
        Task SendPushMessageToGroupsAsync<T>(string[] groups, string collapseKey, RealTimeMessage<T> message,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Get user available groups.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        string[] GetUserAvailableRealTimeGroups(User user);

        #endregion
    }
}