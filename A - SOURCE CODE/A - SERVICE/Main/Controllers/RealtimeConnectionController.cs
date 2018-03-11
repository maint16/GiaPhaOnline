using System;
using System.Net;
using SystemConstant.Enumerations;
using SystemDatabase.Interfaces;
using SystemDatabase.Interfaces.Repositories;
using AutoMapper;
using Main.Constants;
using Main.Interfaces.Services;
using Main.ViewModels.RealtimeConnection;
using Microsoft.AspNetCore.Mvc;
using PusherServer;
using Shared.Interfaces.Services;
using Shared.Models;
using Shared.Resources;

namespace Main.Controllers
{
    [Route("realtime-connection")]
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
                    return StatusCode((int) HttpStatusCode.Forbidden,
                        new ApiResponse(HttpMessages.CannotAccessToPrivateChannel));
            }

            // Authenticate channel.
            authenticationData = _pusherService.Authenticate(info.ChannelName, info.SocketId);
            if (authenticationData == null)
                return StatusCode((int) HttpStatusCode.Forbidden,
                    new ApiResponse(HttpMessages.CannotAuthenticateToChannel));

            #endregion
            
            return Ok(authenticationData);
        }

        #endregion

    }
}