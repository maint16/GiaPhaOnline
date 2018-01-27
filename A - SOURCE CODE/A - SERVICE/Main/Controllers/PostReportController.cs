using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SystemConstant.Enumerations;
using SystemConstant.Enumerations.Order;
using SystemDatabase.Interfaces;
using SystemDatabase.Models.Entities;
using Main.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Services;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels;
using Shared.ViewModels.PostReports;

namespace Main.Controllers
{
    [Route("api/[controller]")]
    public class PostReportController : Controller
    {
        #region Properties

        /// <summary>
        /// Instance which is for accessing database in the system.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Instance for handling identity in request.
        /// </summary>
        private readonly IIdentityService _identityService;

        /// <summary>
        /// Service which is for handling time calculation.
        /// </summary>
        private readonly ITimeService _timeService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize controller with injectors.
        /// </summary>
        public PostReportController(IUnitOfWork unitOfWork, IIdentityService identityService, ITimeService timeService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _timeService = timeService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add post report to the system.
        /// </summary>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<IActionResult> AddPostReport([FromBody] AddPostReportViewModel info)
        {
            #region Parameter validation

            if (info == null)
            {
                info = new AddPostReportViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Find post

            // Find identity.
            var identity = _identityService.GetProfile(HttpContext);

            // Find posts which doesn't belong to the requester.
            var posts = _unitOfWork.RepositoryPosts.Search();
            posts = posts.Where(x => x.Id == info.PostId && x.Status == PostStatus.Active && x.OwnerId != identity.Id);
            var post = await posts.FirstOrDefaultAsync();

            if (post == null)
                return NotFound(new ApiResponse(HttpMessages.PostNotFound));

            #endregion

            #region Check report duplicate

            // Find post reports.
            var postReports = _unitOfWork.RepositoryPostReports.Search();
            postReports = postReports.Where(x => x.PostId == post.Id && x.ReporterId == identity.Id);
            var postReport = await postReports.FirstOrDefaultAsync();

            // Post has been reported.
            if (postReport != null)
                return StatusCode((int)HttpStatusCode.Conflict, new ApiResponse(HttpMessages.PostHasBeenReported));

            #endregion

            #region Post report initialization

            postReport = new PostReport();
            postReport.PostId = post.Id;
            postReport.OwnerId = post.OwnerId;
            postReport.ReporterId = identity.Id;
            postReport.Body = post.Body;
            postReport.Reason = info.Reason;
            postReport.CreatedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Insert post to system.
            _unitOfWork.RepositoryPostReports.Insert(postReport);

            // Commit changes.
            await _unitOfWork.CommitAsync();

            #endregion

            return Ok(postReport);
        }

        /// <summary>
        /// Find post report and edit it.
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPut("")]
        public async Task<IActionResult> EditPostReport([FromQuery] int postId, [FromBody] EditPostReportViewModel info)
        {
            #region Parameters validation

            if (info == null)
            {
                info = new EditPostReportViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Post report search

            // Find identit in request.
            var identity = _identityService.GetProfile(HttpContext);

            // Find post reports.
            var postReports = _unitOfWork.RepositoryPostReports.Search();
            postReports = postReports.Where(x => x.PostId == postId && x.ReporterId == identity.Id && x.Status == PostReportStatus.Active);

            // Find the report.
            var postReport = await postReports.FirstOrDefaultAsync();
            if (postReport == null)
                return NotFound(new ApiResponse(HttpMessages.PostReportNotFound));

            #endregion

            #region Edit post report information

            // Update reason
            postReport.Reason = info.Reason;
            postReport.LastModifiedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Commit changes to system.
            await _unitOfWork.CommitAsync();

            #endregion

            return Ok(postReport);
        }

        /// <summary>
        /// Delete a post report from system.
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        [HttpDelete("")]
        public async Task<IActionResult> DeletePostReport([FromQuery] int postId)
        {
            #region Find post reports

            // Find identity from request.
            var identity = _identityService.GetProfile(HttpContext);
            
            // Find post reports.
            var postReports = _unitOfWork.RepositoryPostReports.Search();
            postReports = postReports.Where(x =>
                x.PostId == postId && x.ReporterId == identity.Id && x.Status == PostReportStatus.Active);

            // Find post report.
            var postReport = await postReports.FirstOrDefaultAsync();
            if (postReport == null)
                return NotFound(new ApiResponse(HttpMessages.PostNotFound));

            #endregion

            #region Update information

            // Update status to deleted.
            postReport.Status = PostReportStatus.Deleted;

            // Commit data.
            await _unitOfWork.CommitAsync();

            #endregion

            return Ok();
        }

        /// <summary>
        /// Search for post reports.
        /// </summary>
        /// <returns></returns>
        [HttpPost("search")]
        public async Task<IActionResult> SearchForPostReports([FromBody] SearchPostReportViewModel condition)
        {
            #region Properties

            if (condition == null)
            {
                condition = new SearchPostReportViewModel();
                TryValidateModel(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Search for post reports

            // Find identity from request.
            var identity = _identityService.GetProfile(HttpContext);

            // Get all post reports.
            var postReports = _unitOfWork.RepositoryPostReports.Search();

            // Post id has been defined.
            if (condition.PostId != null)
                postReports = postReports.Where(x => x.PostId == condition.PostId.Value);

            // Post owner id has been defined.
            if (condition.OwnerId != null)
                postReports = postReports.Where(x => x.OwnerId == condition.OwnerId.Value);

            // Reason is defined.
            if (!string.IsNullOrWhiteSpace(condition.Reason))
                postReports = postReports.Where(x => x.Reason.Contains(condition.Reason));

            // Filter base on role.
            if (identity.Role == AccountRole.Admin)
            {
                // Reporter id has been defined.
                if (condition.ReporterId != null)
                    postReports = postReports.Where(x => x.ReporterId == condition.ReporterId.Value);

                // Statuses have been defined.
                if (condition.Statuses != null && condition.Statuses.Count > 0)
                    postReports = postReports.Where(x => condition.Statuses.Contains(x.Status));
            }
            else
            {
                postReports = postReports.Where(x => x.ReporterId == identity.Id);
                postReports = postReports.Where(x => x.Status == PostReportStatus.Active);
            }

            // Created time has been defined.
            var createdTime = condition.CreatedTime;
            if (createdTime != null)
            {
                var from = createdTime.From;
                var to = createdTime.To;

                if (from != null)
                    postReports = _unitOfWork.RepositoryPostReports.SearchNumericProperty(postReports,
                        x => x.CreatedTime, from.Value, NumericComparision.GreaterEqual);

                if (to != null)
                    postReports = _unitOfWork.RepositoryPostReports.SearchNumericProperty(postReports,
                        x => x.CreatedTime, to.Value, NumericComparision.LowerEqual);
            }

            // Last modified time has been defined.
            var lastModifiedTime = condition.LastModifiedTime;
            if (lastModifiedTime != null)
            {
                var from = lastModifiedTime.From;
                var to = lastModifiedTime.To;

                if (from != null)
                    postReports = _unitOfWork.RepositoryPostReports.SearchNumericProperty(postReports,
                        x => x.LastModifiedTime, from.Value, NumericComparision.GreaterEqual);

                if (to != null)
                    postReports = _unitOfWork.RepositoryPostReports.SearchNumericProperty(postReports,
                        x => x.LastModifiedTime, to.Value, NumericComparision.LowerEqual);
            }

            // Sorting.
            var sort = condition.Sort;
            if (sort != null)
                postReports = _unitOfWork.RepositoryPostReports.Sort(postReports, sort.Direction, sort.Property);
            else
                postReports = _unitOfWork.RepositoryPostReports.Sort(postReports, SortDirection.Decending,
                    PostReportSort.CreatedTime);
            
            #endregion

            #region Result search and count

            // Count post task initialization.
            var pCountPostReports = postReports.CountAsync();
            var pGetPosts = _unitOfWork.Paginate(postReports, condition.Pagination).ToListAsync();

            // Wait for all tasks to complete.
            await Task.WhenAll(pCountPostReports, pGetPosts);

            // Result initialization.
            var result = new SearchResult<IList<PostReport>>();
            result.Records = pGetPosts.Result;
            result.Total = pCountPostReports.Result;

            #endregion

            return Ok(result);
        }
        #endregion
    }
}