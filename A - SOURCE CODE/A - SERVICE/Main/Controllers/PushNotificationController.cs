using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SystemConstant.Enumerations;
using SystemConstant.Enumerations.Order;
using SystemConstant.Models;
using SystemDatabase.Interfaces;
using SystemDatabase.Interfaces.Repositories;
using SystemDatabase.Models.Entities;
using AutoMapper;
using Main.Authentications.ActionFilters;
using Main.Constants;
using Main.Interfaces.Services;
using Main.Models.PushNotification;
using Main.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Services;
using Shared.ViewModels;
using Shared.ViewModels.Categories;
using Shared.ViewModels.Device;

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

        /// <summary>
        ///     Instance for accessing database.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Provide access to generic database functions.
        /// </summary>
        private readonly IDbSharedService _databaseFunction;

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
            _unitOfWork = unitOfWork;
            _databaseFunction = dbSharedService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add device to system.
        /// </summary>
        /// <returns></returns>
        [HttpPost("device")]
        [ByPassAuthorization]
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

        /// <summary>
        /// Search for a list of devices.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("search")]
        public async Task<IActionResult> SearchDevices([FromBody] SearchDeviceViewModel condition)
        {
            #region Parameters validation

            if (condition == null)
            {
                condition = new SearchDeviceViewModel();
                TryValidateModel(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Search for information

            // Get all categories.
            var devices = _unitOfWork.Devices.Search();
            devices = SearchDevices(devices, condition);

            // Sort by properties.
            if (condition.Sort != null)
                devices =
                    _databaseFunction.Sort(devices, condition.Sort.Direction,
                        condition.Sort.Property);
            else
                devices = _databaseFunction.Sort(devices, SortDirection.Decending,
                    DeviceSort.CreatedTime);

            // Result initialization.
            var result = new SearchResult<IList<Device>>();
            result.Total = await devices.CountAsync();
            result.Records = await _databaseFunction.Paginate(devices, condition.Pagination).ToListAsync();

            #endregion

            return Ok(result);
        }

        /// <summary>
        ///     Search devices by using specific conditions.
        /// </summary>
        /// <param name="devices"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public IQueryable<Device> SearchDevices(IQueryable<Device> devices,
            SearchDeviceViewModel conditions)
        {
            if (conditions == null)
                return devices;

            // Id has been defined.
            if (conditions.DeviceId != null)
                devices = devices.Where(x => x.Id == conditions.DeviceId);

            // owner has been defined.
            if (conditions.OwnerId != null)
                devices = devices.Where(x => x.OwnerId == conditions.OwnerId.Value);

            // CreatedTime time range has been defined.
            if (conditions.CreatedTime != null)
            {
                // Start time is defined.
                if (conditions.CreatedTime.From != null)
                    devices = devices.Where(x => x.CreatedTime >= conditions.CreatedTime.From.Value);

                // End time is defined.
                if (conditions.CreatedTime.To != null)
                    devices = devices.Where(x => x.CreatedTime <= conditions.CreatedTime.To.Value);
            }

            return devices;
        }

#if DEBUG

        /// <summary>
        /// Send push notification.
        /// Available in debug mode.
        /// </summary>
        /// <returns></returns>
        [HttpPost("send")]
        [ByPassAuthorization]
        public async Task<IActionResult> Send([FromBody] FcmMessage fcmMessage)
        {
            await _fcmService.SendNotification(fcmMessage);
            return Ok();
        }

        [HttpPost("send-mail")]
        [ByPassAuthorization]
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