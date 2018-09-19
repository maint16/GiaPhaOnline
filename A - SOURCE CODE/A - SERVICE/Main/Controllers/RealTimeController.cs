﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppDb.Interfaces;
using AppDb.Models.Entities;
using AppModel.Enumerations;
using Main.Authentications.ActionFilters;
using Main.Constants.RealTime;
using Main.Hubs;
using Main.Interfaces.Services;
using Main.Interfaces.Services.RealTime;
using Main.ViewModels.RealTime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Services;
using Shared.Resources;

namespace Main.Controllers
{
    [Route("api/real-time")]
    public class RealTimeController : Controller
    {
        #region Properties

        /// <summary>
        /// Instance to manage database connection.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Instance to manage identity
        /// </summary>
        private readonly IIdentityService _identityService;
        
        private readonly ITimeService _timeService;

        private readonly IRealTimeService _realTimeService;

        private readonly ICloudMessagingService _cloudMessagingService;

        #endregion

        #region Constructor

        public RealTimeController(IUnitOfWork unitOfWork, IIdentityService identityService, ITimeService timeService, IRealTimeService realTimeService, ICloudMessagingService cloudMessagingService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _timeService = timeService;
            _realTimeService = realTimeService;
            _cloudMessagingService = cloudMessagingService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Asssign user to push channel which attached to him/her before.
        /// </summary>
        /// <returns></returns>
        [HttpPost("register-device-token")]
        public async Task<IActionResult> AssignPushChannel([FromBody] AssignPushChannelViewModel model)
        {
            #region Model validation

            if (model == null)
            {
                model = new AssignPushChannelViewModel();
                TryValidateModel(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get token information.
            var clientIdToken = await _cloudMessagingService.GetCloudMessagingTokenInformationAsync(model.DeviceId);
            if (clientIdToken == null)
            {
                ModelState.AddModelError($"{nameof(model)}.{nameof(model.DeviceId)}", HttpMessages.DeviceIdInvalid);
                return BadRequest(ModelState);
            }

            #endregion
            
            // Get user identity.
            var profile = _identityService.GetProfile(HttpContext);
            
            // Find all device token that user has.
            var userDeviceTokens = _unitOfWork.UserDeviceTokens.Search();
            userDeviceTokens = userDeviceTokens.Where(x => x.DeviceId == model.DeviceId);

            // Device token is being used by another user.
            if (await userDeviceTokens.AnyAsync(x => x.UserId != profile.Id))
                return Conflict(HttpMessages.DeviceTokenInUse);

            if (await userDeviceTokens.AnyAsync(x => x.UserId == profile.Id))
                return Ok();

            var userDeviceToken = new UserDeviceToken();
            userDeviceToken.DeviceId = model.DeviceId;
            userDeviceToken.UserId = profile.Id;
            userDeviceToken.CreatedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            _unitOfWork.UserDeviceTokens.Insert(userDeviceToken);
            return Ok();
        }

#if DEBUG

        /// <summary>
        /// Send to client.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("send-to-clients")]
        [ByPassAuthorization]
        public async Task<IActionResult> SendMessageToSignalrClients([FromBody] SendMessageToSignalrClientViewModel model)
        {
            if (model == null)
            {
                model = new SendMessageToSignalrClientViewModel();
                TryValidateModel(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _realTimeService.SendToClientsAsync(model.ClientIds, model.EventName, model.Message, CancellationToken.None);
            return Ok();
        }

        /// <summary>
        /// Send to group.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("send-to-groups")]
        [ByPassAuthorization]
        public async Task<IActionResult> SendMessageToSignalrGroups([FromBody] SendMessageToSignalGroupViewModel model)
        {
            if (model == null)
            {
                model = new SendMessageToSignalGroupViewModel();
                TryValidateModel(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _realTimeService.SendToGroupsAsync(model.Groups, model.EventName, model.Message, CancellationToken.None);
            return Ok();
        }

        /// <summary>
        /// Get all registered device token.
        /// </summary>
        /// <returns></returns>
        [HttpGet("device-tokens")]
        [ByPassAuthorization]
        public async Task<IActionResult> GetDeviceTokens()
        {
            var deviceTokens = _unitOfWork.UserDeviceTokens.Search();
            var offlineDeviceTokens = await deviceTokens.ToListAsync();
            return Ok(offlineDeviceTokens);
        }

#endif

#endregion
    }
}