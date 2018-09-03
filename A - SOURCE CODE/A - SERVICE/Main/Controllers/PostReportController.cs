using AppDb.Interfaces;
using AppDb.Interfaces.Repositories;
using Main.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Interfaces.Services;

namespace Main.Controllers
{
    [Route("api/[controller]")]
    public class PostReportController : Controller
    {
        #region Constructors

        /// <summary>
        ///     Initialize controller with injectors.
        /// </summary>
        public PostReportController(IUnitOfWork unitOfWork, IIdentityService identityService, ITimeService timeService,
            IRelationalDbService databaseFunction)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _timeService = timeService;
            _databaseFunction = databaseFunction;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Instance which is for accessing database in the system.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        ///     Instance for handling identity in request.
        /// </summary>
        private readonly IIdentityService _identityService;

        /// <summary>
        ///     Service which is for handling time calculation.
        /// </summary>
        private readonly ITimeService _timeService;

        /// <summary>
        ///     Service which is for accessing database function.
        /// </summary>
        private readonly IRelationalDbService _databaseFunction;

        #endregion

        //#region Methods

        ///// <summary>
        ///// Add post report to the system.
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost("")]
        //public async Task<IActionResult> AddPostReport([FromBody] AddPostReportViewModel info)
        //{
        //    #region Parameter validation

        //    if (info == null)
        //    {
        //        info = new AddPostReportViewModel();
        //        TryValidateModel(info);
        //    }

        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    #endregion

        //    #region Find post

        //    // Find identity.
        //    var identity = _identityService.GetProfile(HttpContext);

        //    // Find posts which doesn't belong to the requester.
        //    var posts = _unitOfWork.Posts.Search();
        //    posts = posts.Where(x => x.Id == info.PostId && x.Status == PostStatus.Available && x.OwnerId != identity.Id);
        //    var post = await posts.FirstOrDefaultAsync();

        //    if (post == null)
        //        return NotFound(new ApiResponse(HttpMessages.PostNotFound));

        //    #endregion

        //    #region Check report duplicate

        //    // Find post reports.
        //    var postReports = _unitOfWork.PostReports.Search();
        //    postReports = postReports.Where(x => x.PostId == post.Id && x.ReporterId == identity.Id);
        //    var postReport = await postReports.FirstOrDefaultAsync();

        //    // Post has been reported.
        //    if (postReport != null)
        //        return StatusCode((int)HttpStatusCode.Conflict, new ApiResponse(HttpMessages.PostHasBeenReported));

        //    #endregion

        //    #region Post report initialization

        //    postReport = new PostReport();
        //    postReport.PostId = post.Id;
        //    postReport.OwnerId = post.OwnerId;
        //    postReport.ReporterId = identity.Id;
        //    postReport.Body = post.Body;
        //    postReport.Reason = info.Reason;
        //    postReport.CreatedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

        //    // Insert post to system.
        //    _unitOfWork.PostReports.Insert(postReport);

        //    // Commit changes.
        //    await _unitOfWork.CommitAsync();

        //    #endregion

        //    return Ok(postReport);
        //}

        ///// <summary>
        ///// Find post report and edit it.
        ///// </summary>
        ///// <param name="postId"></param>
        ///// <param name="info"></param>
        ///// <returns></returns>
        //[HttpPut("")]
        //public async Task<IActionResult> EditPostReport([FromRoute] int postId, [FromBody] EditPostReportViewModel info)
        //{
        //    #region Parameters validation

        //    if (info == null)
        //    {
        //        info = new EditPostReportViewModel();
        //        TryValidateModel(info);
        //    }

        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    #endregion

        //    #region Post report search

        //    // Find identit in request.
        //    var identity = _identityService.GetProfile(HttpContext);

        //    // Find post reports.
        //    var postReports = _unitOfWork.PostReports.Search();
        //    postReports = postReports.Where(x => x.PostId == postId && x.ReporterId == identity.Id && x.Status == PostReportStatus.Available);

        //    // Find the report.
        //    var postReport = await postReports.FirstOrDefaultAsync();
        //    if (postReport == null)
        //        return NotFound(new ApiResponse(HttpMessages.PostReportNotFound));

        //    #endregion

        //    #region Edit post report information

        //    // Update reason
        //    postReport.Reason = info.Reason;
        //    postReport.LastModifiedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

        //    // Commit changes to system.
        //    await _unitOfWork.CommitAsync();

        //    #endregion

        //    return Ok(postReport);
        //}

        ///// <summary>
        ///// Delete a post report from system.
        ///// </summary>
        ///// <param name="postId"></param>
        ///// <returns></returns>
        //[HttpDelete("")]
        //public async Task<IActionResult> DeletePostReport([FromRoute] int postId)
        //{
        //    #region Find post reports

        //    // Find identity from request.
        //    var identity = _identityService.GetProfile(HttpContext);

        //    // Find post reports.
        //    var postReports = _unitOfWork.PostReports.Search();
        //    postReports = postReports.Where(x =>
        //        x.PostId == postId && x.ReporterId == identity.Id && x.Status == PostReportStatus.Available);

        //    // Find post report.
        //    var postReport = await postReports.FirstOrDefaultAsync();
        //    if (postReport == null)
        //        return NotFound(new ApiResponse(HttpMessages.PostNotFound));

        //    #endregion

        //    #region Update information

        //    // Update status to deleted.
        //    postReport.Status = PostReportStatus.Deleted;

        //    // Commit data.
        //    await _unitOfWork.CommitAsync();

        //    #endregion

        //    return Ok();
        //}

        ///// <summary>
        ///// Search for post reports.
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost("search")]
        //public async Task<IActionResult> SearchForPostReports([FromBody] SearchPostReportViewModel condition)
        //{
        //    #region Properties

        //    if (condition == null)
        //    {
        //        condition = new SearchPostReportViewModel();
        //        TryValidateModel(condition);
        //    }

        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    #endregion

        //    #region Search for post reports

        //    // Find identity from request.
        //    var identity = _identityService.GetProfile(HttpContext);

        //    // Get all post reports.
        //    var postReports = _unitOfWork.PostReports.Search();

        //    // Post id has been defined.
        //    if (condition.PostId != null)
        //        postReports = postReports.Where(x => x.PostId == condition.PostId.Value);

        //    // Post owner id has been defined.
        //    if (condition.OwnerId != null)
        //        postReports = postReports.Where(x => x.OwnerId == condition.OwnerId.Value);

        //    // Reason is defined.
        //    if (!string.IsNullOrWhiteSpace(condition.Reason))
        //        postReports = postReports.Where(x => x.Reason.Contains(condition.Reason));

        //    // Filter base on role.
        //    if (identity.Role == AccountRole.Admin)
        //    {
        //        // Reporter id has been defined.
        //        if (condition.ReporterId != null)
        //            postReports = postReports.Where(x => x.ReporterId == condition.ReporterId.Value);

        //        // Statuses have been defined.
        //        if (condition.Statuses != null && condition.Statuses.Count > 0)
        //            postReports = postReports.Where(x => condition.Statuses.Contains(x.Status));
        //    }
        //    else
        //    {
        //        postReports = postReports.Where(x => x.ReporterId == identity.Id);
        //        postReports = postReports.Where(x => x.Status == PostReportStatus.Available);
        //    }

        //    // Created time has been defined.
        //    var createdTime = condition.CreatedTime;
        //    if (createdTime != null)
        //    {
        //        var from = createdTime.From;
        //        var to = createdTime.To;

        //        if (from != null)
        //            postReports = _databaseFunction.SearchNumericProperty(postReports,
        //                x => x.CreatedTime, from.Value, NumericComparision.GreaterEqual);

        //        if (to != null)
        //            postReports = _databaseFunction.SearchNumericProperty(postReports,
        //                x => x.CreatedTime, to.Value, NumericComparision.LowerEqual);
        //    }

        //    // Last modified time has been defined.
        //    var lastModifiedTime = condition.LastModifiedTime;
        //    if (lastModifiedTime != null)
        //    {
        //        var from = lastModifiedTime.From;
        //        var to = lastModifiedTime.To;

        //        if (from != null)
        //            postReports = _databaseFunction.SearchNumericProperty(postReports,
        //                x => x.LastModifiedTime, from.Value, NumericComparision.GreaterEqual);

        //        if (to != null)
        //            postReports = _databaseFunction.SearchNumericProperty(postReports,
        //                x => x.LastModifiedTime, to.Value, NumericComparision.LowerEqual);
        //    }

        //    // Sorting.
        //    var sort = condition.Sort;
        //    if (sort != null)
        //        postReports = _databaseFunction.Sort(postReports, sort.Direction, sort.Property);
        //    else
        //        postReports = _databaseFunction.Sort(postReports, SortDirection.Decending,
        //            PostReportSort.CreatedTime);

        //    #endregion

        //    #region Result search and count

        //    // Result initialization.
        //    var result = new SearchResult<IList<PostReport>>();

        //    result.Total = await postReports.CountAsync();
        //    result.Records = await _databaseFunction.Paginate(postReports, condition.Pagination).ToListAsync();
        //    #endregion

        //    return Ok(result);
        //}
        //#endregion
    }
}