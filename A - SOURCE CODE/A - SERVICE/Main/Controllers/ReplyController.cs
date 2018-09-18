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
using Main.ViewModels.Reply;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Services;
using Shared.Models;
using Shared.Resources;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Main.Controllers
{
    [Route("api/[controller]")]
    public class ReplyController : ApiBaseController
    {
        #region Constructors

        public ReplyController(
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
        ///     Add reply to system.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<IActionResult> AddReply([FromBody] AddReplyViewModel info)
        {
            #region Parameters validation

            if (info == null)
            {
                info = new AddReplyViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Find topic

            // Find all topics.
            var topics = UnitOfWork.Topics.Search();
            topics = topics.Where(x => x.Id == info.TopicId && x.Status == ItemStatus.Active);

            // Check whether topic exists or not.
            var bIsTopicAvailable = await topics.AnyAsync();
            if (!bIsTopicAvailable)
                return NotFound(new ApiResponse(HttpMessages.TopicNotFound));

            #endregion

            #region Reply initialization

            // Find identity from request.
            var identity = IdentityService.GetProfile(HttpContext);

            // Reply intialization.
            var reply = new Reply();
            reply.OwnerId = identity.Id;
            reply.TopicId = info.TopicId;
            reply.CategoryId = info.CategoryId;
            reply.CategoryGroupId = info.CategoryGroupId;
            reply.Content = info.Content;
            reply.Status = ItemStatus.Active;
            reply.CreatedTime = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);
            reply.LastModifiedTime = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Insert reply into system.
            UnitOfWork.Replies.Insert(reply);

            await UnitOfWork.CommitAsync();

            #endregion

            return Ok(reply);
        }

        /// <summary>
        /// Edit reply by using specific information.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditReply([FromRoute] int id, [FromBody] EditReplyViewModel info)
        {
            #region Parameters validation

            if (info == null)
            {
                info = new EditReplyViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Find reply

            // Get request identity.
            var identity = IdentityService.GetProfile(HttpContext);

            // Get all replies in database.
            var replies = UnitOfWork.Replies.Search();

            replies = replies.Where(x => x.Id == id && x.Status == ItemStatus.Active);

            // Get the first matched reply.
            var reply = await replies.FirstOrDefaultAsync();
            if (reply == null)
                return NotFound(new ApiResponse(HttpMessages.ReplyNotFound));

            #endregion

            #region Update reply information

            // Check whether information has been updated or not.
            var bHasInformationChanged = false;

            // Content is defined
            if (info.Content != null && info.Content != reply.Content)
            {
                reply.Content = info.Content;
                bHasInformationChanged = true;
            }

            // Status is defined.
            if (info.Status != reply.Status)
            {
                reply.Status = info.Status;
                bHasInformationChanged = true;
            }

            if (bHasInformationChanged)
            {
                reply.LastModifiedTime = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);

                // Commit changes to database.
                await UnitOfWork.CommitAsync();
            }

            #endregion

            return Ok(reply);
        }

        /// <summary>
        ///     Load reply by using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("search")]
        public async Task<IActionResult> LoadReplies([FromBody] SearchReplyViewModel condition)
        {
            #region Parameters validation

            if (condition == null)
            {
                condition = new SearchReplyViewModel();
                TryValidateModel(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            // Find identity in request.
            var identity = IdentityService.GetProfile(HttpContext);

            #region Search for information

            // Get all reply
            var replies = _unitOfWork.Replies.Search();

            // Id have been defined.
            if (condition.Ids != null && condition.Ids.Count > 0)
            {
                condition.Ids = condition.Ids.Where(x => x > 0).ToList();
                if (condition.Ids != null && condition.Ids.Count > 0)
                {
                    replies = replies.Where(x => condition.Ids.Contains(x.Id));
                }
            }

            // Topic Id have been defined.
            if (condition.TopicIds != null && condition.TopicIds.Count > 0)
            {
                condition.TopicIds = condition.TopicIds.Where(x => x > 0).ToList();
                if (condition.TopicIds != null && condition.TopicIds.Count > 0)
                {
                    replies = replies.Where(x => condition.TopicIds.Contains(x.TopicId));
                }
            }

            // Category id have been defined
            if (condition.CategoryIds != null && condition.CategoryIds.Count > 0)
            {
                condition.CategoryIds = condition.CategoryIds.Where(x => x > 0).ToList();
                if (condition.CategoryIds != null && condition.CategoryIds.Count > 0)
                {
                    replies = replies.Where(x => condition.CategoryIds.Contains(x.CategoryId));
                }
            }

            // Category group Id have been defined.
            if (condition.CategoryGroupIds != null && condition.CategoryGroupIds.Count > 0)
            {
                condition.CategoryGroupIds = condition.CategoryGroupIds.Where(x => x > 0).ToList();
                if (condition.CategoryGroupIds != null && condition.CategoryGroupIds.Count > 0)
                {
                    replies = replies.Where(x => condition.CategoryGroupIds.Contains(x.CategoryGroupId));
                }
            }

            // Owner Id have been defined.
            if (condition.OwnerIds != null && condition.OwnerIds.Count > 0)
            {
                condition.OwnerIds = condition.OwnerIds.Where(x => x > 0).ToList();
                if (condition.OwnerIds != null && condition.OwnerIds.Count > 0)
                {
                    replies = replies.Where(x => condition.OwnerIds.Contains(x.OwnerId));
                }
            }

            // Content have been defined.
            if (condition.Contents != null && condition.Contents.Count > 0)
            {
                condition.Contents = condition.Contents.Where(x => !string.IsNullOrEmpty(x)).ToList();
                if (condition.Contents != null && condition.Contents.Count > 0)
                {
                    replies = replies.Where(x => condition.Contents.Any(y => x.Content.Contains(y)));
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
                        replies = replies.Where(x => condition.Statuses.Contains(x.Status));
                }
            }

            #endregion

            // Sort by properties.
            if (condition.Sort != null)
                replies =
                    _databaseFunction.Sort(replies, condition.Sort.Direction,
                        condition.Sort.Property);
            else
                replies = _databaseFunction.Sort(replies, SortDirection.Decending,
                    ReplySort.Id);

            // Result initialization.
            var result = new SearchResult<IList<Reply>>();
            result.Total = await replies.CountAsync();
            result.Records = await _databaseFunction.Paginate(replies, condition.Pagination).ToListAsync();

            return Ok(result);
        }

        #endregion
    }
}
