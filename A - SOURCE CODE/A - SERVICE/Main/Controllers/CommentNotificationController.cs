using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemConstant.Enumerations;
using SystemConstant.Enumerations.Order;
using SystemDatabase.Interfaces;
using SystemDatabase.Interfaces.Repositories;
using SystemDatabase.Models.Entities;
using AutoMapper;
using Main.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Services;
using Shared.ViewModels;
using Shared.ViewModels.CommentNotification;

namespace Main.Controllers
{
    public class CommentNotificationController : ApiBaseController
    {
        #region Constructors

        /// <summary>
        /// Initialize controller with injectors.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="mapper"></param>
        /// <param name="timeService"></param>
        /// <param name="dbSharedService"></param>
        /// <param name="identityService"></param>
        public CommentNotificationController(IUnitOfWork unitOfWork, IMapper mapper, ITimeService timeService, IDbSharedService dbSharedService, IIdentityService identityService) : base(unitOfWork, mapper, timeService, dbSharedService, identityService)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Search for comment notification
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] SearchCommentNotificationViewModel condition)
        {
            #region Parameters validation

            if (condition == null)
            {
                condition = new SearchCommentNotificationViewModel();
                TryValidateModel(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Search for information

            // Get requester profile.
            var profile = IdentityService.GetProfile(HttpContext);

            // Get all comment notifications.
            var commentNotifications = UnitOfWork.CommentNotifications.Search();

            // Id is defined.
            if (condition.Id != null)
                commentNotifications = commentNotifications.Where(x => x.Id == condition.Id);

            // Comment id is defined.
            if (condition.CommentId != null)
                commentNotifications = commentNotifications.Where(x => x.CommentId == condition.CommentId.Value);

            // Post id is defined.
            if (condition.PostId != null)
                commentNotifications = commentNotifications.Where(x => x.PostId == condition.PostId);
            
            // User can only see his/her broadcasted or received notification.
            commentNotifications =
                commentNotifications.Where(x => x.RecipientId == profile.Id || x.BroadcasterId == profile.Id);

            // Types are defined.
            if (condition.Types != null)
            {
                condition.Types = condition.Types.Where(x => Enum.IsDefined(typeof(NotificationType), x)).ToHashSet();
                if (condition.Types.Count > 0)
                    commentNotifications = commentNotifications.Where(x => condition.Types.Contains(x.Type));
            }

            // Statuses are defined.
            if (condition.Statuses != null)
            {
                condition.Statuses = condition.Statuses.Where(x => Enum.IsDefined(typeof(NotificationStatus), x)).ToHashSet();
                if (condition.Statuses.Count > 0)
                    commentNotifications = commentNotifications.Where(x => condition.Statuses.Contains(x.Status));
            }

            // Sorting.
            var sort = condition.Sort;
            if (sort != null)
                commentNotifications = DbSharedService.Sort(commentNotifications, sort.Direction, sort.Property);
            else
                commentNotifications = DbSharedService.Sort(commentNotifications, SortDirection.Decending,
                    CommentNotificationSort.CreatedTime);

            // Count total records.
            var iTotalRecords = await commentNotifications.CountAsync();
            
            // Paginate records.
            var records = await DbSharedService.Paginate(commentNotifications, condition.Pagination).ToListAsync();

            var result = new SearchResult<IList<CommentNotification>>();
            result.Records = records;
            result.Total = iTotalRecords;

            #endregion

            return Ok(result);

        }

        #endregion
    }
}