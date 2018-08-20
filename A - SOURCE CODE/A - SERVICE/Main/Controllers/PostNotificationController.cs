using AppDb.Interfaces;
using AppDb.Interfaces.Repositories;
using AutoMapper;
using Main.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Interfaces.Services;

namespace Main.Controllers
{
    [Route("api/post-notification")]
    public class PostNotificationController : ApiBaseController
    {
        #region Constructors

        /// <summary>
        ///     Initialize controller with injectors.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="mapper"></param>
        /// <param name="timeService"></param>
        /// <param name="relationalDbService"></param>
        /// <param name="identityService"></param>
        public PostNotificationController(IUnitOfWork unitOfWork, IMapper mapper, ITimeService timeService,
            IRelationalDbService relationalDbService, IIdentityService identityService) : base(unitOfWork, mapper, timeService,
            relationalDbService, identityService)
        {
        }

        #endregion

        //#region Methods

        ///// <summary>
        ///// Search for post notifications.
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost("search")]
        //public async Task<IActionResult> Search([FromBody] SearchPostNotificationViewModel condition)
        //{
        //    #region Parameters validation

        //    if (condition == null)
        //    {
        //        condition = new SearchPostNotificationViewModel();
        //        TryValidateModel(condition);
        //    }

        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    #endregion

        //    #region Search for information

        //    // Get requester profile.
        //    var profile = IdentityService.GetProfile(HttpContext);

        //    // Get all comment notifications.
        //    var postNotifications = UnitOfWork.PostNotifications.Search();

        //    // Id is defined.
        //    if (condition.Id != null)
        //        postNotifications = postNotifications.Where(x => x.Id == condition.Id);

        //    // Comment id is defined.
        //    if (condition.PostId != null)
        //        postNotifications = postNotifications.Where(x => x.PostId == condition.PostId.Value);

        //    // User can only see his/her broadcasted or received notification.
        //    postNotifications =
        //        postNotifications.Where(x => x.RecipientId == profile.Id || x.BroadcasterId == profile.Id);

        //    // Types are defined.
        //    if (condition.Types != null)
        //    {
        //        condition.Types = condition.Types.Where(x => Enum.IsDefined(typeof(NotificationType), x)).ToHashSet();
        //        if (condition.Types.Count > 0)
        //            postNotifications = postNotifications.Where(x => condition.Types.Contains(x.Type));
        //    }

        //    // Statuses are defined.
        //    if (condition.Statuses != null)
        //    {
        //        condition.Statuses = condition.Statuses.Where(x => Enum.IsDefined(typeof(NotificationStatus), x)).ToHashSet();
        //        if (condition.Statuses.Count > 0)
        //            postNotifications = postNotifications.Where(x => condition.Statuses.Contains(x.Status));
        //    }

        //    // Sorting.
        //    var sort = condition.Sort;
        //    if (sort != null)
        //        postNotifications = RelationalDbService.Sort(postNotifications, sort.Direction, sort.Property);
        //    else
        //        postNotifications = RelationalDbService.Sort(postNotifications, SortDirection.Decending,
        //            CommentNotificationSort.CreatedTime);

        //    // Count total records.
        //    var iTotalRecords = await postNotifications.CountAsync();

        //    // Paginate records.
        //    var records = await RelationalDbService.Paginate(postNotifications, condition.Pagination).ToListAsync();

        //    var result = new SearchResult<IList<PostNotification>>();
        //    result.Records = records;
        //    result.Total = iTotalRecords;

        //    #endregion

        //    return Ok(result);
        //}

        //#endregion
    }
}