using AppDb.Interfaces;
using AppDb.Interfaces.Repositories;
using AppDb.Models.Entities;
using AutoMapper;
using Main.Hubs;
using Main.Interfaces;
using Main.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Shared.Interfaces.Services;
using VgySdk.Interfaces;

namespace Main.Controllers
{
    [Route("api/[controller]")]
    public class CategoryController : Controller
    {
        #region Constructors

        /// <summary>
        ///     Initialize controller with injectors.
        /// </summary>
        /// <param name="identityService">Service which is for handling identity.</param>
        /// <param name="timeService">Service which is for handling time calculation.</param>
        /// <param name="unitOfWork">Instance for accessing database.</param>
        /// <param name="databaseFunction"></param>
        /// <param name="pushService"></param>
        /// <param name="mapper">Instance for mapping objects</param>
        /// <param name="vgyService"></param>
        /// <param name="logger"></param>
        /// <param name="categoryCacheService"></param>
        /// <param name="realTimeNotificationService"></param>
        /// <param name="notificationHubContext"></param>
        /// <param name="notifyService"></param>
        public CategoryController(IIdentityService identityService, ITimeService timeService, IUnitOfWork unitOfWork,
            IRelationalDbService databaseFunction,
            IPushService pushService,
            IMapper mapper, IVgyService vgyService, ILogger<UserController> logger,
            IValueCacheService<int, Category> categoryCacheService,
            IRealTimeNotificationService realTimeNotificationService,
            IHubContext<NotificationHub> notificationHubContext,
            INotifyService notifyService)
        {
            _identityService = identityService;
            _timeService = timeService;
            _unitOfWork = unitOfWork;
            _databaseFunction = databaseFunction;
            _pushService = pushService;
            _mapper = mapper;
            _vgyService = vgyService;
            _logger = logger;
            _categoryCacheService = categoryCacheService;
            _realTimeNotificationService = realTimeNotificationService;
            _notificationHubContext = notificationHubContext;
            _notifyService = notifyService;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Service which is for handling identity.
        /// </summary>
        private readonly IIdentityService _identityService;

        /// <summary>
        ///     Service which is for handling time calculation.
        /// </summary>
        private readonly ITimeService _timeService;

        /// <summary>
        ///     Instance for accessing database.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        ///     Instance for mapping objects.
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        ///     Provide access to generic database functions.
        /// </summary>
        private readonly IRelationalDbService _databaseFunction;

        /// <summary>
        ///     Instance to send push notification to clients.
        /// </summary>
        private readonly IPushService _pushService;

        /// <summary>
        ///     Service which is for handling file upload to vgy.me hosting.
        /// </summary>
        private readonly IVgyService _vgyService;

        /// <summary>
        ///     Logging instance.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        ///     Category cache
        /// </summary>
        private readonly IValueCacheService<int, Category> _categoryCacheService;

        /// <summary>
        ///     Realtime notification
        /// </summary>
        private readonly IRealTimeNotificationService _realTimeNotificationService;

        /// <summary>
        /// </summary>
        private readonly IHubContext<NotificationHub> _notificationHubContext;

        /// <summary>
        /// </summary>
        private readonly INotifyService _notifyService;

        #endregion
    }
}