﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SystemConstant.Enumerations;
using SystemDatabase.Interfaces;
using SystemDatabase.Interfaces.Repositories;
using SystemDatabase.Models.Entities;
using AutoMapper;
using Main.Constants;
using Main.Hubs;
using Main.Interfaces.Services;
using Main.ViewModels.RealtimeConnection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PusherServer;
using Shared.Interfaces.Services;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.SignalrConnections;

namespace Main.Controllers
{
    [Route("api/realtime-connection")]
    public class RealtimeConnectionController : ApiBaseController
    {
        #region Properties

        /// <summary>
        /// Pusher service to send realtime data.
        /// </summary>
        private readonly IPusherService _pusherService;

        private readonly IRealTimeNotificationService _realTimeNotificationService;

        private readonly IHubContext<NotificationHub> _notificationHubContext;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize controller with injectors.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="mapper"></param>
        /// <param name="timeService"></param>
        /// <param name="dbSharedService"></param>
        /// <param name="identityService"></param>
        /// <param name="pusherService"></param>
        /// <param name="realTimeNotificationService"></param>
        /// <param name="notificationHubContext"></param>
        public RealtimeConnectionController(IUnitOfWork unitOfWork, IMapper mapper, ITimeService timeService,
            IDbSharedService dbSharedService, IIdentityService identityService,
            IPusherService pusherService, IRealTimeNotificationService realTimeNotificationService,
            IHubContext<NotificationHub> notificationHubContext) : base(unitOfWork, mapper, timeService, dbSharedService, identityService)
        {
            _pusherService = pusherService;
            _realTimeNotificationService = realTimeNotificationService;
            _notificationHubContext = notificationHubContext;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Authorize SignalR connection.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost("signalr/authorize")]
        public async Task<IActionResult> AuthorizeSignalR([FromBody] AuthorizeSignalrViewModel info)
        {
            #region Parameters validation

            if (info == null)
            {
                info = new AuthorizeSignalrViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            // Get requester identity.
            var profile = IdentityService.GetProfile(HttpContext);

            #region Connection existence check

            // Find real-time connections.
            var realTimeConnections = UnitOfWork.SignalrConnections.Search();
            realTimeConnections = realTimeConnections.Where(x => x.Id.Equals(info.Id));

            // Find the first connection.
            var realTimeConnection = await realTimeConnections.FirstOrDefaultAsync();
            if (realTimeConnection != null)
            {
                // Owner is different.
                if (realTimeConnection.OwnerId != profile.Id)
                {
                    realTimeConnection.OwnerId = profile.Id;
                    await UnitOfWork.CommitAsync();
                }

                return Ok(realTimeConnection);
            }

            #endregion

            #region Connection initialization

            // Initialize real-time connection.
            realTimeConnection = new SignalrConnection();
            realTimeConnection.Id = info.Id;
            realTimeConnection.OwnerId = profile.Id;
            realTimeConnection.CreatedTime = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Save connection information into system.
            UnitOfWork.SignalrConnections.Insert(realTimeConnection);

            // Save changes.
            await UnitOfWork.CommitAsync();

            #endregion

            return Ok(realTimeConnection);
        }

#if DEBUG

        /// <summary>
        /// Send pusher notification
        /// </summary>
        /// <returns></returns>
        [HttpPost("pusher/send")]
        [AllowAnonymous]
        public async Task<IActionResult> SendPusher([FromBody] SendPusherMessageViewModel information)
        {
            #region Parameters validation

            if (information == null)
            {
                information = new SendPusherMessageViewModel();
                TryValidateModel(information);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            var triggerResult = await _pusherService.SendAsync(information.SocketId, information.ChannelName, information.EventName,
                 information.Information);

            return Ok();
        }
        
        /// <summary>
        /// Broadcast signalr notification to all clients.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("signalr/send")]
        public async Task<IActionResult> BroadcastSignalrNotification([FromBody] BroadcastSignalrNotificationViewModel info)
        {
            if (info == null)
            {
                info = new BroadcastSignalrNotificationViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Find client.
            IClientProxy clientProxy;
            if (info.Clients != null && info.Clients.Count > 0)
            {
                var clientIds = info.Clients.ToList();
                clientProxy = _notificationHubContext.Clients.Clients(clientIds);
            }
            else
                clientProxy = _notificationHubContext.Clients.All;

            await _realTimeNotificationService.BroadcastAsync(clientProxy, info.MethodName, info.Data);
            return Ok();
        }

#endif
        #endregion
    }
}