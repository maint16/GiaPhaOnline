using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Main.Interfaces.Services;
using Main.Models;
using Microsoft.AspNetCore.SignalR;

namespace Main.Services
{
    public class RealTimeNotificationService : IRealTimeNotificationService
    {
        #region Methods

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="clientProxy"></param>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Task BroadcastAsync(IClientProxy clientProxy, string methodName, object parameters)
        {
            // Client proxy is invalid.
            if (clientProxy == null)
                throw new Exception("Client(s) is/are invalid.");

            // Method name is undefined.
            if (string.IsNullOrWhiteSpace(methodName))
                throw new Exception("Method name must be defined");
            
            return clientProxy.SendAsync(methodName, parameters);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="hubContext"></param>
        /// <param name="connectionIds"></param>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Task BroadcastAsync<T>(IHubContext<T> hubContext, IReadOnlyList<string> connectionIds, string methodName, object parameters = null) where T:Hub
        {
            // Hub context isn't initialized.
            if (hubContext == null)
                throw new Exception("Hub context isn't initialized");

            // Invalid connection indexes are invalid.
            if (connectionIds == null || connectionIds.Count < 1)
                throw new Exception("Connection indexes list is null or empty");

            IClientProxy clientProxy;
#if DEBUG
            clientProxy = hubContext.Clients.All;
#else
            // Get client proxy.
             clientProxy = hubContext.Clients.Clients(connectionIds);
#endif
            return BroadcastAsync(clientProxy, methodName, parameters);
        }

#endregion
    }
}