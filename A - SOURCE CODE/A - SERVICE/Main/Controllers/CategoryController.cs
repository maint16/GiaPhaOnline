using System;
using System.Collections.Generic;
using System.IO;
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
using Main.Hubs;
using Main.Interfaces.Services;
using Main.Models;
using Main.Models.PushNotification;
using Main.Models.PushNotification.Notification;
using Main.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Enumerations;
using Shared.Interfaces.Services;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels;
using Shared.ViewModels.Accounts;
using Shared.ViewModels.Categories;
using SkiaSharp;
using VgySdk.Interfaces;
using VgySdk.Models;

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
        public CategoryController(IIdentityService identityService, ITimeService timeService, IUnitOfWork unitOfWork,
            IDbSharedService databaseFunction,
            IPushService pushService,
            IMapper mapper, IVgyService vgyService, ILogger<UserController> logger,
            IValueCacheService<int, Category> categoryCacheService,
            IRealTimeNotificationService realTimeNotificationService,
            IHubContext<NotificationHub> notificationHubContext)
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
        /// Provide access to generic database functions.
        /// </summary>
        private readonly IDbSharedService _databaseFunction;

        /// <summary>
        /// Instance to send push notification to clients.
        /// </summary>
        private readonly IPushService _pushService;

        /// <summary>
        /// Service which is for handling file upload to vgy.me hosting.
        /// </summary>
        private readonly IVgyService _vgyService;

        /// <summary>
        /// Logging instance.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Category cache
        /// </summary>
        private readonly IValueCacheService<int, Category> _categoryCacheService;

        /// <summary>
        /// Realtime notification
        /// </summary>
        private readonly IRealTimeNotificationService _realTimeNotificationService;

        /// <summary>
        /// 
        /// </summary>
        private readonly IHubContext<NotificationHub> _notificationHubContext;

        #endregion

        #region Methods

        /// <summary>
        ///     Find a specific category by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ByPassAuthorization]
        public async Task<IActionResult> FindCategory([FromRoute] int id)
        {
            // Find category.
            var categories = _unitOfWork.Categories.Search();
            categories = categories.Where(x => x.Id == id);

            // Find the first matched result.
            var category = await categories.FirstOrDefaultAsync();

            // Cannot find the category.
            if (category == null)
                return NotFound(new ApiResponse(HttpMessages.CategoryNotFound));

            return Ok(category);
        }

        /// <summary>
        ///     Add a category into database.
        /// </summary>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<IActionResult> AddCategory([FromBody] AddCategoryViewModel info)
        {
            #region Parameters validation

            if (info == null)
            {
                info = new AddCategoryViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            //#region Image proccessing

            //var photo = info.Photo.Split(",");
            //var binaryPhoto = Convert.FromBase64String(photo[1]);
            //var memoryStream = new MemoryStream(binaryPhoto);
            //var skManagedStream = new SKManagedStream(memoryStream);
            //var skBitmap = SKBitmap.Decode(skManagedStream);
            //var resizedSkBitmap = skBitmap.Resize(new SKImageInfo(512, 512), SKBitmapResizeMethod.Lanczos3);

            //#endregion

            #region Category initialization

            // Find requester identity.
            var profile = _identityService.GetProfile(Request.HttpContext);

            // Initialize category.
            var category = new Category();
            category.CreatorId = profile.Id;
            category.Status = ItemStatus.Available;
            category.Name = info.Name;
            category.CreatedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Add category into database.
            _unitOfWork.Categories.Insert(category);

            // Commit changes.
            await _unitOfWork.CommitAsync();

            #region Broadcast real-time notification

            // Find accounts have admin role
            var accounts = _unitOfWork.Accounts.Search();
            accounts = accounts.Where(x => x.Role == AccountRole.Admin);

            // Get all connection indexes.
            var signalrConnections = _unitOfWork.SignalrConnections.Search();

            // Get all devices to send push notification to.
            var devices = _unitOfWork.Devices.Search();
            
            var connectionIds = (from signalrConnection in signalrConnections
                                 join account in accounts on signalrConnection.OwnerId equals account.Id
                                 select signalrConnection.Id).ToList();

            // At least one connection has been found.
            if (connectionIds.Count > 0)
            {

                // Additional data.
                var additionalInfo = new Dictionary<string, object>();
                additionalInfo.Add("creator", profile.Nickname);
                additionalInfo.Add("category", category);

                // Initialize notification to broadcast to clients.
                var realTimeNotification =
                    new RealTimeNotification(NotificationCategory.Category, NotificationAction.Add, additionalInfo);
                
                await _realTimeNotificationService.BroadcastAsync(_notificationHubContext, connectionIds, HubMethodConstant.ClientReceiveNotification, realTimeNotification);
            }

            #endregion

            #region Send push notification
            
            //fcmMessage.
            //_pushService.SendNotification(new)

            #endregion

            #endregion

            return Ok(category);
        }

        /// <summary>
        ///     Edit a category by search for its index.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCategory([FromRoute] int id, [FromBody] EditCategoryViewModel info)
        {
            #region Parameters validations

            if (info == null)
            {
                info = new EditCategoryViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Find category

            // Find category.
            var categories = _unitOfWork.Categories.Search();
            categories = categories.Where(x => x.Id == id);

            // Find category.
            var category = await categories.FirstOrDefaultAsync();
            if (category == null)
                return NotFound(new ApiResponse(HttpMessages.CategoryNotFound));

            #endregion

            #region Information update

            // Whether information has been changed or not.
            var bHasInformationChanged = false;

            // Name is specified.
            if (info.Name != null)
            {
                category.Name = info.Name;
                bHasInformationChanged = true;
            }

            // Status is specified.
            if (info.Status != null)
            {
                category.Status = info.Status.Value;
                bHasInformationChanged = true;
            }

            // Photo is defined.
            if (!string.IsNullOrEmpty(info.Photo))
            {
                var photo = info.Photo.Split(",");
                var binaryPhoto = Convert.FromBase64String(photo[1]);
                var memoryStream = new MemoryStream(binaryPhoto);
                var skManagedStream = new SKManagedStream(memoryStream);
                var skBitmap = SKBitmap.Decode(skManagedStream);
                var resizedSkBitmap = skBitmap.Resize(new SKImageInfo(512, 512), SKBitmapResizeMethod.Lanczos3);

                bHasInformationChanged = true;
            }

            // Commit changes to database.
            if (bHasInformationChanged)
                await _unitOfWork.CommitAsync();

            #endregion

            return Ok();
        }

        /// <summary>
        ///     Search for a list of categories.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("search")]
        [ByPassAuthorization]
        public async Task<IActionResult> SearchCategories([FromBody] SearchCategoryViewModel condition)
        {
            #region Parameters validation

            if (condition == null)
            {
                condition = new SearchCategoryViewModel();
                TryValidateModel(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Search for information

            // Get all categories.
            var categories = _unitOfWork.Categories.Search();
            categories = SearchCategories(categories, condition);

            // Sort by properties.
            if (condition.Sort != null)
                categories =
                    _databaseFunction.Sort(categories, condition.Sort.Direction,
                        condition.Sort.Property);
            else
                categories = _databaseFunction.Sort(categories, SortDirection.Decending,
                    CategoriesSort.CreatedTime);

            // Result initialization.
            var result = new SearchResult<IList<Category>>();
            result.Total = await categories.CountAsync();
            result.Records = await _databaseFunction.Paginate(categories, condition.Pagination).ToListAsync();

            #endregion

            return Ok(result);
        }

        /// <summary>
        ///     Search categories by using specific conditions.
        /// </summary>
        /// <param name="categories"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public IQueryable<Category> SearchCategories(IQueryable<Category> categories,
            SearchCategoryViewModel conditions)
        {
            if (conditions == null)
                return categories;

            // Id has been defined.
            if (conditions.Id != null)
                categories = categories.Where(x => x.Id == conditions.Id.Value);

            // Creator has been defined.
            if (conditions.CreatorId != null)
                categories = categories.Where(x => x.CreatorId == conditions.CreatorId.Value);

            // Name search condition has been defined.
            if (conditions.Name != null && !string.IsNullOrWhiteSpace(conditions.Name))
                categories = _databaseFunction.SearchPropertyText(categories, x => x.Name,
                    new TextSearch(TextSearchMode.ContainIgnoreCase, conditions.Name));

            // CreatedTime time range has been defined.
            if (conditions.CreatedTime != null)
            {
                // Start time is defined.
                if (conditions.CreatedTime.From != null)
                    categories = categories.Where(x => x.CreatedTime >= conditions.CreatedTime.From.Value);

                // End time is defined.
                if (conditions.CreatedTime.To != null)
                    categories = categories.Where(x => x.CreatedTime <= conditions.CreatedTime.To.Value);
            }

            // Last modified time range has been defined.
            if (conditions.LastModifiedTime != null)
            {
                // Start time is defined.
                if (conditions.LastModifiedTime.From != null)
                    categories = categories.Where(x => x.LastModifiedTime >= conditions.LastModifiedTime.From.Value);

                // End time is defined.
                if (conditions.LastModifiedTime.To != null)
                    categories = categories.Where(x => x.LastModifiedTime <= conditions.LastModifiedTime.To.Value);
            }

            return categories;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("load-categories")]
        public async Task<IActionResult> LoadCategories([FromBody] LoadCategoryViewModel condition)
        {
            #region Parameters validation

            if (condition == null)
            {
                condition = new LoadCategoryViewModel();
                TryValidateModel(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            var result = new SearchResult<IList<Category>>();

            #region Search for information

            // Get all category
            var categories = _unitOfWork.Categories.Search();

            // List of account that will be returned.

            // Get all valid items in cache.
            var cacheValues = _categoryCacheService.ReadValues();
            var filteredCategories = cacheValues.Where(x => condition.Ids.Contains(x.Id)).ToList();
            result.Records = filteredCategories;

            // Get remaining categories that aren't in cache.
            condition.Ids = condition.Ids.Where(x => x > 0 && filteredCategories.All(y => y.Id != x)).ToList();

            // At least 1 element has been found.
            if (condition.Ids.Count > 0)
            {
                // Get all remaining items from database to memory.
                // Later, all these items will be added to cache.
                var inMemoryCategories = await categories.Where(x => condition.Ids.Contains(x.Id)).ToListAsync();

                foreach (var inMemoryCategory in inMemoryCategories)
                    _categoryCacheService.Add(inMemoryCategory.Id, inMemoryCategory, LifeTimeConstant.CategoryCacheLifeTime);

                result.Records = filteredCategories.Concat(inMemoryCategories).ToList();
            }

            // Result initialization.
            result.Total = result.Records.Count;

            #endregion

            return Ok(result);
        }

        /// <summary>
        /// Upload Photo
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost("upload-photo")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadPhoto([FromBody] UploadCategoryPhotoViewModel info)
        {
            #region Parameters Validation

            if (info == null)
            {
                info = new UploadCategoryPhotoViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            // Get requester profile.
            var profile = _identityService.GetProfile(HttpContext);

            #region Image proccessing

            // Reflect image variable.
            var image = info.Photo;

            using (var skManagedStream = new SKManagedStream(image.OpenReadStream()))
            {
                var skBitmap = SKBitmap.Decode(skManagedStream);

                try
                {
                    // Resize image to 512x512 size.
                    var resizedSkBitmap = skBitmap.Resize(new SKImageInfo(512, 512), SKBitmapResizeMethod.Lanczos3);

                    // Initialize file name.
                    var fileName = $"{Guid.NewGuid():D}.png";

                    using (var skImage = SKImage.FromBitmap(resizedSkBitmap))
                    using (var skData = skImage.Encode(SKEncodedImageFormat.Png, 100))
                    using (var memoryStream = new MemoryStream())
                    {
                        skData.SaveTo(memoryStream);
                        var vgySuccessRespone = await _vgyService.UploadAsync<VgySuccessResponse>(
                            memoryStream.ToArray(),
                            image.ContentType, fileName,
                            CancellationToken.None);

                        // Response is empty.
                        if (vgySuccessRespone == null || vgySuccessRespone.IsError)
                            return StatusCode(StatusCodes.Status403Forbidden,
                                new ApiResponse(HttpMessages.ImageIsInvalid));

                        var category = new Category
                        {
                            PhotoRelativeUrl = vgySuccessRespone.ImageUrl,
                            PhotoAbsoluteUrl = vgySuccessRespone.ImageDeleteUrl
                        };

                        _unitOfWork.Categories.Insert(category);
                    }

                    // Save changes into database.
                    await _unitOfWork.CommitAsync();

                    return Ok(profile);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception.Message, exception);
                    return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse(HttpMessages.ImageIsInvalid));
                }
            }

            #endregion
        }
        #endregion
    }
}