using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppBusiness.Interfaces;
using AppDb.Interfaces;
using AppDb.Models.Entities;
using Main.Authentications.ActionFilters;
using Main.Interfaces.Services;
using Main.Interfaces.Services.RealTime;
using Main.Models.PushNotification;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceShared.Interfaces.Services;
using Shared.Resources;
using Shared.ViewModels.RealTime;

namespace Main.Controllers
{
    [Route("api/real-time")]
    public class RealTimeController : Controller
    {
        #region Constructor

        public RealTimeController(IUnitOfWork unitOfWork, IProfileService identityService, ITimeService timeService,
            IRealTimeService realTimeService, ICloudMessagingService cloudMessagingService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _timeService = timeService;
            _realTimeService = realTimeService;
            _cloudMessagingService = cloudMessagingService;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Instance to manage database connection.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        ///     Instance to manage identity
        /// </summary>
        private readonly IProfileService _identityService;

        private readonly ITimeService _timeService;

        private readonly IRealTimeService _realTimeService;

        private readonly ICloudMessagingService _cloudMessagingService;

        #endregion

        #region Methods

        /// <summary>
        ///     Asssign user to push channel which attached to him/her before.
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
            var profile = _identityService.GetProfile();

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
            await _unitOfWork.CommitAsync();
            return Ok();
        }

#if DEBUG

        /// <summary>
        ///     Send to client.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("send-to-clients")]
        [ByPassAuthorization]
        public async Task<IActionResult> SendMessageToSignalrClients(
            [FromBody] SendMessageToSignalrClientViewModel model)
        {
            if (model == null)
            {
                model = new SendMessageToSignalrClientViewModel();
                TryValidateModel(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _realTimeService.SendRealTimeMessageToClientsAsync(model.ClientIds, model.EventName, model.Message,
                CancellationToken.None);
            return Ok();
        }

        [HttpPost("push-to-groups")]
        [ByPassAuthorization]
        public async Task<IActionResult> PushToGroup([FromBody] FcmMessage<Dictionary<string, object>> model)
        {
            if (model == null)
            {
                model = new FcmMessage<Dictionary<string, object>>();
                TryValidateModel(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _cloudMessagingService.SendAsync(model, CancellationToken.None);
            return Ok();
        }

        /// <summary>
        ///     Send to group.
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

            await _realTimeService.SendRealTimeMessageToGroupsAsync(model.Groups, model.EventName, model.Message,
                CancellationToken.None);
            return Ok();
        }

        /// <summary>
        ///     Get all registered device token.
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

        /// <summary>
        ///     Add device to specific group.
        /// </summary>
        /// <returns></returns>
        [HttpPost("add-device-to-group")]
        public async Task<IActionResult> AddDeviceToGroup([FromBody] AddDeviceToGroupViewModel model)
        {
            if (model == null)
            {
                model = new AddDeviceToGroupViewModel();
                TryValidateModel(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var profile = _identityService.GetProfile();

            using (var transaction = _unitOfWork.BeginTransactionScope())
            {
                var userDevices = _unitOfWork.UserDeviceTokens.Search();
                userDevices = userDevices.Where(x => (x.DeviceId == model.DeviceId) & (x.UserId == profile.Id));
                _unitOfWork.UserDeviceTokens.Remove(userDevices);

                var userRealTimeGroups = _unitOfWork.UserRealTimeGroups.Search();
                userRealTimeGroups = userRealTimeGroups.Where(x => x.UserId == profile.Id && x.Group == model.Group);
                _unitOfWork.UserRealTimeGroups.Remove(userRealTimeGroups);

                var userDeviceToken = new UserDeviceToken();
                userDeviceToken.DeviceId = model.DeviceId;
                userDeviceToken.UserId = profile.Id;
                _unitOfWork.UserDeviceTokens.Insert(userDeviceToken);

                var userRealTimeGroup = new UserRealTimeGroup();
                userRealTimeGroup.Id = new Guid();
                userRealTimeGroup.UserId = profile.Id;
                userRealTimeGroup.Group = model.Group;
                userRealTimeGroup.CreatedTime = 0;
                _unitOfWork.UserRealTimeGroups.Insert(userRealTimeGroup);

                await _unitOfWork.CommitAsync();
                transaction.Commit();
            }
            return Ok();
        }
#endif

        #endregion
    }
}