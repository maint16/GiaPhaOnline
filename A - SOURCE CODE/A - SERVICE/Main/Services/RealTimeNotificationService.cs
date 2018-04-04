using System;
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

        #endregion
    }
}