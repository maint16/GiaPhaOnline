using System;
using System.Linq;
using System.Threading.Tasks;
using SystemDatabase.Interfaces;
using SystemDatabase.Models.Entities;
using Main.Authentications.Requirements;
using Main.Constants;
using Main.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Interfaces.Services;

namespace Main.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
        Policy = PolicyConstant.DefaultSignalRPolicyName)]
    public class NotificationHub : Hub
    {
        #region Properties

        /// <summary>
        /// Service to handle identity in incoming request.
        /// </summary>
        private readonly IIdentityService _identityService;

        /// <summary>
        /// Service which is for handling profile cache.
        /// </summary>
        private readonly IRealTimeConnectionCacheService _realTimeConnectionCacheService;

        /// <summary>
        /// Instance to resolve DI.
        /// </summary>
        private readonly IServiceProvider _serviceProvider;
        #endregion

        #region Constructor

        /// <summary>
        /// Initialize hub with injectors.
        /// </summary>
        public NotificationHub(IIdentityService identityService, IRealTimeConnectionCacheService realTimeConnectionCacheService, IServiceProvider serviceProvider)
        {
            _identityService = identityService;
            _realTimeConnectionCacheService = realTimeConnectionCacheService;
            _serviceProvider = serviceProvider;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called when user connected to hub.
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            // Find http context.
            var httpContext = Context.Connection.GetHttpContext();
            if (httpContext == null)
                return;

            // Get profile in request.
            var user = _identityService.GetProfile(httpContext);
            if (user == null)
                return;
            
            // Check whether connection string has been saved into cache or not.
            var connection = _realTimeConnectionCacheService.Read(Context.ConnectionId);

            // Already in cache.
            if (connection != null)
                return;

            // Not in cache. Find the connection in the database. If the connection doesnt exist in database, initialize it.
            // Get instance of UnitOfWork.
            var unitOfWork = _serviceProvider.GetService<IUnitOfWork>();
            
            // Get date time service.
            var timeService = _serviceProvider.GetService<ITimeService>();

            // Find the realtime connection in database.
            var realTimeConnections = unitOfWork.SignalrConnections.Search();
            realTimeConnections = realTimeConnections.Where(x => x.OwnerId == user.Id);

            // Find the first connection.
            var realTimeConnection = await realTimeConnections.FirstOrDefaultAsync();
            if (realTimeConnection != null)
            {
                // Add to cache.
                _realTimeConnectionCacheService.Add(Context.ConnectionId, user);
                return;
            }

            // Add this connection to cache.
            _realTimeConnectionCacheService.Add(Context.ConnectionId, user);

            // Initialize a new connection into database.
            realTimeConnection = new SignalrConnection();
            realTimeConnection.Id = Context.ConnectionId;
            realTimeConnection.OwnerId = user.Id;
            realTimeConnection.CreatedTime = timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            unitOfWork.SignalrConnections.Insert(realTimeConnection);
            unitOfWork.Commit();

            return;
        }

        #endregion
    }
}