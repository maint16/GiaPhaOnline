using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SystemDatabase.Interfaces;
using SystemDatabase.Interfaces.Repositories;
using SystemDatabase.Models.Entities;
using AutoMapper;
using Main.Constants;
using Main.Interfaces.Services;
using Main.Models.PushNotification;
using Main.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Services;

namespace Main.Controllers
{
    [Route("api/push-notification")]
    public class PushNotificationController : ApiBaseController
    {
        #region Properties

        /// <summary>
        /// Firebase cloud messaging service.
        /// </summary>
        private readonly IPushNotificationService _fcmService;

        /// <summary>
        /// Email caching service
        /// </summary>
        private readonly IEmailCacheService _emailCacheService;

        /// <summary>
        /// Service which is for sending mail.
        /// </summary>
        private readonly ISendMailService _sendMailService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize controller with injectors.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="mapper"></param>
        /// <param name="timeService"></param>
        /// <param name="dbSharedService"></param>
        /// <param name="identityService"></param>
        /// <param name="fcmService"></param>
        /// <param name="emailCacheService"></param>
        /// <param name="sendMailService"></param>
        public PushNotificationController(IUnitOfWork unitOfWork, IMapper mapper, ITimeService timeService, IDbSharedService dbSharedService,
            IIdentityService identityService, IPushNotificationService fcmService,
            IEmailCacheService emailCacheService, ISendMailService sendMailService) : base(unitOfWork, mapper, timeService, dbSharedService, identityService)
        {
            _fcmService = fcmService;
            _emailCacheService = emailCacheService;
            _sendMailService = sendMailService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add device to system.
        /// </summary>
        /// <returns></returns>
        [HttpPost("")]
        [AllowAnonymous]
        public async Task<IActionResult> AddDevice([FromBody] AddDeviceViewModel info)
        {
            #region Parameters validation

            // Information hasn't been initialized.
            if (info == null)
            {
                info = new AddDeviceViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Find device & update information.

            // Flag to check whether information has been changed or not.
            var bHasInformationChanged = false;

            // Find request identity.
            var identity = IdentityService.GetProfile(HttpContext);

            // Find a list of devices.
            var devices = UnitOfWork.Devices.Search();
            devices = devices.Where(x => x.Id.Equals(info.DeviceId, StringComparison.InvariantCultureIgnoreCase));

            // Find the first device in system.
            var device = await devices.FirstOrDefaultAsync();

            // Device is not available. Initialize it.
            if (device == null)
            {
                device = new Device();
                device.Id = info.DeviceId;

                // User is authenticated. Update the identity.
                if (identity != null)
                    device.OwnerId = identity.Id;

                device.CreatedTime = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);
                UnitOfWork.Devices.Insert(device);
            }
            else
            {
                // Owner is defined.
                if (device.OwnerId != identity.Id)
                {
                    device.OwnerId = identity.Id;
                    bHasInformationChanged = true;
                }
            }

            #endregion

            // Save changes into system
            if (bHasInformationChanged)
                await UnitOfWork.CommitAsync();
            return Ok();
        }

#if DEBUG

        /// <summary>
        /// Send push notification.
        /// Available in debug mode.
        /// </summary>
        /// <returns></returns>
        [HttpPost("send")]
        [AllowAnonymous]
        public async Task<IActionResult> Send([FromBody] FcmMessage fcmMessage)
        {
            await _fcmService.SendNotification(fcmMessage);
            return Ok();
        }

        [HttpPost("send-mail")]
        [AllowAnonymous]
        public async Task<IActionResult> SendMail()
        {
            var option = _emailCacheService.Read(EmailTemplateConstant.SubmitPasswordRequest);
            await _sendMailService.SendAsync(new HashSet<string>() { "lightalakanzam@gmail.com" }, null, null, "Hello world",
                "Content", false, CancellationToken.None);
            return Ok();
        }

#endif

        #endregion
    }
}