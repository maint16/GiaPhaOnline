using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDb.Interfaces;
using AppDb.Models.Entities;
using AppModel.Enumerations;
using AppModel.Enumerations.Order;
using AutoMapper;
using Main.Interfaces.Services;
using Main.ViewModels.Topic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Services;
using Shared.Models;
using Shared.Resources;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Main.Controllers
{
    [Route("api/[controller]")]
    public class TopicController : ApiBaseController
    {
        #region Constructors

        public TopicController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ITimeService timeService,
            IRelationalDbService relationalDbService,
            IEncryptionService encryptionService,
            IIdentityService identityService) : base(unitOfWork, mapper, timeService,
            relationalDbService, identityService)
        {
            _unitOfWork = unitOfWork;
            _databaseFunction = relationalDbService;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Instance for accessing database.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        ///     Provide access to generic database functions.
        /// </summary>
        private readonly IRelationalDbService _databaseFunction;

        #endregion

        #region Methods

        /// <summary>
        ///     Add topic to system.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<IActionResult> AddTopic([FromBody] AddTopicViewModel info)
        {
            #region Parameters validation

            if (info == null)
            {
                info = new AddTopicViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Find topic

            // Find category.
            var categories = UnitOfWork.Categories.Search();
            categories = categories.Where(x => x.Id == info.CategoryId && x.Status == ItemStatus.Active);

            // Check whether category exists or not.
            var bIsCategoryAvailable = await categories.AnyAsync();
            if (!bIsCategoryAvailable)
                return NotFound(new ApiResponse(HttpMessages.CategoryNotFound));

            #endregion

            #region Topic initialization

            // Find identity from request.
            var identity = IdentityService.GetProfile(HttpContext);

            // Topic intialization.
            var topic = new Topic();
            topic.OwnerId = identity.Id;
            topic.CategoryId = info.CategoryId;
            topic.CategoryGroupId = info.CategoryGroupId;
            topic.Title = info.Title;
            topic.Body = info.Body;
            topic.Status = ItemStatus.Active;
            topic.CreatedTime = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);
            topic.LastModifiedTime = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Insert topic into system.
            UnitOfWork.Topics.Insert(topic);

            await UnitOfWork.CommitAsync();

            #endregion

            return Ok(topic);
        }

        /// <summary>
        /// Edit topic by using specific information.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditTopic([FromRoute] int id, [FromBody] EditTopicViewModel info)
        {
            #region Parameters validation

            if (info == null)
            {
                info = new EditTopicViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Find topic

            // Get request identity.
            var identity = IdentityService.GetProfile(HttpContext);

            // Get all topics in database.
            var topics = UnitOfWork.Topics.Search();

            topics = topics.Where(x => x.Id == id && x.Status == ItemStatus.Active);

            // Get the first matched topic.
            var topic = await topics.FirstOrDefaultAsync();
            if (topic == null)
                return NotFound(new ApiResponse(HttpMessages.TopicNotFound));

            #endregion

            #region Update topic information

            // Check whether information has been updated or not.
            var bHasInformationChanged = false;

            // Category id is defined
            if (info.CategoryId != topic.CategoryId)
            {
                topic.CategoryId = info.CategoryId;
                bHasInformationChanged = true;
            }

            // Category group id is defined
            if (info.CategoryGroupId != topic.CategoryGroupId)
            {
                topic.CategoryGroupId = info.CategoryGroupId;
                bHasInformationChanged = true;
            }

            // Title is defined
            if (info.Title != null && info.Title != topic.Title)
            {
                topic.Title = info.Title;
                bHasInformationChanged = true;
            }

            // Body is defined
            if (info.Body != null && info.Body != topic.Body)
            {
                topic.Body = info.Body;
                bHasInformationChanged = true;
            }

            // Status is defined.
            if (info.Status != topic.Status)
            {
                topic.Status = info.Status;
                bHasInformationChanged = true;
            }

            if (bHasInformationChanged)
            {
                topic.LastModifiedTime = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);

                // Commit changes to database.
                await UnitOfWork.CommitAsync();
            }

            #endregion

            return Ok(topic);
        }

        /// <summary>
        ///     Load topic by using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("search")]
        public async Task<IActionResult> LoadTopics([FromBody] SearchTopicViewModel condition)
        {
            #region Parameters validation

            if (condition == null)
            {
                condition = new SearchTopicViewModel();
                TryValidateModel(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            // Find identity in request.
            var identity = IdentityService.GetProfile(HttpContext);

            #region Search for information

            // Get all topic
            var topics = _unitOfWork.Topics.Search();

            // Id have been defined.
            if (condition.Ids != null && condition.Ids.Count > 0)
            {
                condition.Ids = condition.Ids.Where(x => x > 0).ToList();
                if (condition.Ids != null && condition.Ids.Count > 0)
                {
                    topics = topics.Where(x => condition.Ids.Contains(x.Id));
                }
            }

            // Category id have been defined
            if (condition.CategoryIds != null && condition.CategoryIds.Count > 0)
            {
                condition.CategoryIds = condition.CategoryIds.Where(x => x > 0).ToList();
                if (condition.CategoryIds != null && condition.CategoryIds.Count > 0)
                {
                    topics = topics.Where(x => condition.CategoryIds.Contains(x.CategoryId));
                }
            }

            // Category group Id have been defined.
            if (condition.CategoryGroupIds != null && condition.CategoryGroupIds.Count > 0)
            {
                condition.CategoryGroupIds = condition.CategoryGroupIds.Where(x => x > 0).ToList();
                if (condition.CategoryGroupIds != null && condition.CategoryGroupIds.Count > 0)
                {
                    topics = topics.Where(x => condition.CategoryGroupIds.Contains(x.CategoryGroupId));
                }
            }

            // Owner Id have been defined.
            if (condition.OwnerIds != null && condition.OwnerIds.Count > 0)
            {
                condition.OwnerIds = condition.OwnerIds.Where(x => x > 0).ToList();
                if (condition.OwnerIds != null && condition.OwnerIds.Count > 0)
                {
                    topics = topics.Where(x => condition.OwnerIds.Contains(x.OwnerId));
                }
            }

            // Title have been defined.
            if (condition.Titles != null && condition.Titles.Count > 0)
            {
                condition.Titles = condition.Titles.Where(x => !string.IsNullOrEmpty(x)).ToList();
                if (condition.Titles != null && condition.Titles.Count > 0)
                {
                    topics = topics.Where(x => condition.Titles.Any(y => x.Title.Contains(y)));
                }
            }

            // Body have been defined.
            if (condition.Bodies != null && condition.Bodies.Count > 0)
            {
                condition.Bodies = condition.Bodies.Where(x => !string.IsNullOrEmpty(x)).ToList();
                if (condition.Bodies != null && condition.Bodies.Count > 0)
                {
                    topics = topics.Where(x => condition.Bodies.Any(y => x.Body.Contains(y)));
                }
            }

            // Search conditions which are based on roles.

            if (identity?.Role == UserRole.Admin)
            {
                // Statuses have been defined.
                if (condition.Statuses != null && condition.Statuses.Count > 0)
                {
                    condition.Statuses =
                        condition.Statuses.Where(x => Enum.IsDefined(typeof(ItemStatus), x)).ToList();
                    if (condition.Statuses.Count > 0)
                        topics = topics.Where(x => condition.Statuses.Contains(x.Status));
                }
            }

            #endregion

            // Sort by properties.
            if (condition.Sort != null)
                topics =
                    _databaseFunction.Sort(topics, condition.Sort.Direction,
                        condition.Sort.Property);
            else
                topics = _databaseFunction.Sort(topics, SortDirection.Decending,
                    TopicSort.Title);

            // Result initialization.
            var result = new SearchResult<IList<Topic>>();
            result.Total = await topics.CountAsync();
            result.Records = await _databaseFunction.Paginate(topics, condition.Pagination).ToListAsync();

            return Ok(result);
        }

        #endregion
    }
}
