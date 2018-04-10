using System;
using Main.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;

namespace Main.Hubs
{
    public class NotificationHub : AuthorizedBaseHub
    {
        #region Constructor

        /// <summary>
        /// Initialize hub with injectors.
        /// </summary>
        /// <param name="identityService"></param>
        /// <param name="realTimeConnectionCacheService"></param>
        /// <param name="serviceProvider"></param>
        public NotificationHub(IIdentityService identityService, IRealTimeConnectionCacheService realTimeConnectionCacheService, IServiceProvider serviceProvider) : base(identityService, realTimeConnectionCacheService, serviceProvider)
        {
        }

        #endregion
    }
}