using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDb.Interfaces;
using AppDb.Models.Entities;
using AppModel.Enumerations;
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
    public class AuthorizedBaseHub : Hub
    {
        #region Properties

        /// <summary>
        /// Service to handle identity in incoming request.
        /// </summary>
        protected readonly IIdentityService IdentityService;

        /// <summary>
        /// Service which is for handling profile cache.
        /// </summary>
        protected readonly IRealTimeConnectionCacheService RealTimeConnectionCacheService;

        /// <summary>
        /// Service to manage push notification.
        /// </summary>
        protected readonly IPushService PushService;

        /// <summary>
        /// Instance to resolve DI.
        /// </summary>
        protected readonly IServiceProvider ServiceProvider;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize hub with injectors.
        /// </summary>
        public AuthorizedBaseHub(IIdentityService identityService, IRealTimeConnectionCacheService realTimeConnectionCacheService, IServiceProvider serviceProvider)
        {
            IdentityService = identityService;
            RealTimeConnectionCacheService = realTimeConnectionCacheService;
            ServiceProvider = serviceProvider;
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
            var httpContext = Context.GetHttpContext();
            if (httpContext == null)
                return;

            // Get profile in request.
            var user = IdentityService.GetProfile(httpContext);
            if (user == null)
                return;

            // Check whether connection string has been saved into cache or not.
            var connection = RealTimeConnectionCacheService.Read(Context.ConnectionId);

            // Already in cache.
            if (connection != null)
                return;

            // Not in cache. Find the connection in the database. If the connection doesnt exist in database, initialize it.
            // Get instance of UnitOfWork.
            var unitOfWork = ServiceProvider.GetService<IUnitOfWork>();

            // Get date time service.
            var timeService = ServiceProvider.GetService<ITimeService>();

            // Find the realtime connection in database.
            var realTimeConnections = unitOfWork.SignalrConnections.Search();
            realTimeConnections = realTimeConnections.Where(x => x.Id.Equals(Context.ConnectionId));

            // Find the first connection.
            var realTimeConnection = await realTimeConnections.FirstOrDefaultAsync();
            if (realTimeConnection != null)
            {
                // Add to cache.
                RealTimeConnectionCacheService.Add(Context.ConnectionId, user);
                return;
            }

            // Add this connection to cache.
            RealTimeConnectionCacheService.Add(Context.ConnectionId, user);

            // Initialize a new connection into database.
            realTimeConnection = new SignalrConnection();
            realTimeConnection.Id = Context.ConnectionId;
            realTimeConnection.OwnerId = user.Id;
            realTimeConnection.CreatedTime = timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            unitOfWork.SignalrConnections.Insert(realTimeConnection);

            // List of tasks which should be completed.
            var tasks = new List<Task>();
            Task pAddConnectionToDbTask = unitOfWork.CommitAsync();
            tasks.Add(pAddConnectionToDbTask);

            // Base on user role to add him/her to specific group.
            switch (user.Role)
            {
                case AccountRole.Admin:
                    // Add user to group task.
                    var pAddUserToAdminGroupTask = Groups.AddToGroupAsync(Context.ConnectionId, RealTimeGroupConstant.Admin);
                    tasks.Add(pAddUserToAdminGroupTask);

                    // Add user to ordinary user.
                    var pAddUserToUserGroupTask = Groups.AddToGroupAsync(Context.ConnectionId, RealTimeGroupConstant.User);
                    tasks.Add(pAddUserToUserGroupTask);
                    break;
            }

            // Wait for all task to complete.
            Task.WaitAll(tasks.ToArray());
            return;
        }
        
        #endregion
    }
}