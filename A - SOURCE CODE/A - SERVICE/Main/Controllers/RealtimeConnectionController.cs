using System;
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
using Main.Interfaces.Services;
using Main.ViewModels.RealtimeConnection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PusherServer;
using Shared.Interfaces.Services;
using Shared.Models;
using Shared.Resources;

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
        public RealtimeConnectionController(IUnitOfWork unitOfWork, IMapper mapper, ITimeService timeService,
            IDbSharedService dbSharedService, IIdentityService identityService, IPusherService pusherService) : base(unitOfWork, mapper, timeService, dbSharedService, identityService)
        {
            _pusherService = pusherService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Authorize pusher connection
        /// </summary>
        /// <returns></returns>
        /// <param name="info">Pusher connection information.</param>
        [HttpPost("pusher/authorize")]
        public IActionResult AuthorizePusher([FromBody] AuthorizePusherViewModel info)
        {
            #region Parameters validation

            if (info == null)
            {
                info = new AuthorizePusherViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Get & authorize identity

            // Get profile information.
            var profile = IdentityService.GetProfile(HttpContext);

            // Authentication result.
            IAuthenticationData authenticationData = null;

            // Channel is private.
            if (profile.Role != AccountRole.Admin)
            {
                if (info.ChannelName.StartsWith("private-", StringComparison.InvariantCultureIgnoreCase))
                    return StatusCode((int)HttpStatusCode.Forbidden,
                        new ApiResponse(HttpMessages.CannotAccessToPrivateChannel));
            }

            // Authenticate channel.
            authenticationData = _pusherService.Authenticate(info.ChannelName, info.SocketId);
            if (authenticationData == null)
                return StatusCode((int)HttpStatusCode.Forbidden,
                    new ApiResponse(HttpMessages.CannotAuthenticateToChannel));

#if DEBUG
            Debug.WriteLine($"Authenticated socket id: {info.ChannelName} into channel ${info.SocketId}. Authentication data: {authenticationData.auth}");
#endif
            #endregion

            return Ok(authenticationData);
        }

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

           var triggerResult = await  _pusherService.SendAsync(information.SocketId, information.ChannelName, information.EventName,
                information.Information);

            return Ok();
        }

#endif
        #endregion

    }
}