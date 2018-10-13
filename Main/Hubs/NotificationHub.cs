using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppDb.Interfaces;
using AppDb.Models.Entities;
using Main.Constants;
using Main.Interfaces.Services;
using Main.Interfaces.Services.RealTime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Shared.Interfaces.Services;

namespace Main.Hubs
{
    [Authorize(PolicyConstant.DefaultSignalRPolicyName)]
    public class NotificationHub : Hub
    {
        #region Constructor

        /// <summary>
        ///     Initialize hub with injectors.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="identityService"></param>
        /// <param name="timeService"></param>
        /// <param name="realTimeService"></param>
        public NotificationHub(IUnitOfWork unitOfWork, IProfileService identityService, ITimeService timeService,
            IRealTimeService realTimeService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _timeService = timeService;
            _realTimeService = realTimeService;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Unit of work.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        private readonly IProfileService _identityService;

        private readonly ITimeService _timeService;

        private readonly IRealTimeService _realTimeService;

        private static readonly ConcurrentDictionary<string, List<string>> UserGroups =
            new ConcurrentDictionary<string, List<string>>();

        #endregion

        #region Methods

        /// <summary>
        ///     Called when a client connects to hub.
        /// </summary>
        /// <returns></returns>
        public override Task OnConnectedAsync()
        {
            // Get connection id.
            var connectionId = Context.ConnectionId;
            Debug.WriteLine($"Client {connectionId} has connected to {nameof(NotificationHub)}");

            #region Save connection id to database

            // Get profle
            var profile = _identityService.GetProfile(Context.GetHttpContext());

            // Check whether connection id has been saved to this user.
            var signalrConnections = _unitOfWork.SignalrConnections.Search();
            signalrConnections = signalrConnections.Where(x => x.ClientId == connectionId);
            var signalrConnection = signalrConnections.FirstOrDefault();
            if (signalrConnection == null)
            {
                signalrConnection = new SignalrConnection();
                signalrConnection.ClientId = connectionId;
                signalrConnection.LastActivityTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);
                signalrConnection.UserId = profile.Id;
                _unitOfWork.SignalrConnections.Insert(signalrConnection);
            }
            else
            {
                signalrConnection.UserId = profile.Id;
            }

            _unitOfWork.Commit();

            #endregion

            #region Add connection to group

            // Add connection to a specific groups.
            var availableGroups = _realTimeService.GetUserAvailableRealTimeGroups(profile).ToList();

            // Initialize background tasks.
            var addClientToGroupTasks = new List<Task>();
            foreach (var group in availableGroups)
            {
                var addClientToGroupTask = Groups.AddToGroupAsync(Context.ConnectionId, group);
                addClientToGroupTasks.Add(addClientToGroupTask);
            }

            Task.WhenAll(addClientToGroupTasks.ToArray());
            UserGroups.TryAdd(connectionId, availableGroups);

            #endregion

            return base.OnConnectedAsync();
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override Task OnDisconnectedAsync(Exception exception)
        {
            // Get connection id.
            var connectionId = Context.ConnectionId;

            Debug.WriteLine($"Client {connectionId} has disconnected from {nameof(NotificationHub)}");

            // Find & remove the disconnected connection.
            var signalrConnections = _unitOfWork.SignalrConnections.Search();
            signalrConnections = signalrConnections.Where(x => x.ClientId == connectionId);
            _unitOfWork.SignalrConnections.Remove(signalrConnections);
            _unitOfWork.Commit();

            // Get all groups that client takes part in.
            var groups = new List<string>();
            UserGroups.TryGetValue(connectionId, out groups);
            if (groups != null)
            {
                var deleteGroupTasks = new List<Task>();
                foreach (var group in groups)
                {
                    var deleteGroupTask =
                        Groups.RemoveFromGroupAsync(Context.ConnectionId, group, CancellationToken.None);
                    deleteGroupTasks.Add(deleteGroupTask);
                }

                Task.WhenAll(deleteGroupTasks.ToArray());
            }
            return base.OnDisconnectedAsync(exception);
        }

        #endregion
    }
}