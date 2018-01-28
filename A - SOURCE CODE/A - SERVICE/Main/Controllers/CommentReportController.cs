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
using Shared.ViewModels.CommentReports;
using Shared.ViewModels.PostReports;

namespace Main.Controllers
{
    [Route("api/[controller]")]
    public class CommentReportController : Controller
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
        public CommentReportController(IUnitOfWork unitOfWork, IIdentityService identityService, ITimeService timeService)
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
        public async Task<IActionResult> AddCommentReport([FromBody] AddCommentReportViewModel info)
        {
            #region Parameter validation

            if (info == null)
            {
                info = new AddCommentReportViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Check whether comment belongs to requester or not

            // Find identity.
            var identity = _identityService.GetProfile(HttpContext);

            // Find comments.
            var comments = _unitOfWork.RepositoryComments.Search();
            comments = comments.Where(x =>
                x.Id == info.CommentId && x.OwnerId != identity.Id && x.Status == CommentStatus.Available);

            // Find comment which matches to the search condition.
            var comment = await comments.FirstOrDefaultAsync();
            if (comment == null)
                return NotFound(new ApiResponse(HttpMessages.CommentNotFound));

            #endregion

            #region Check post status

            // Search for posts.
            var posts = _unitOfWork.RepositoryPosts.Search();
            posts = posts.Where(x => x.Id == comment.PostId && x.Status == PostStatus.Available);
            var post = await posts.FirstOrDefaultAsync();
            if (post == null)
                return NotFound(new ApiResponse(HttpMessages.PostNotFound));

            #endregion

            #region Check report duplicate

            // Find comment reports.
            var commentReports = _unitOfWork.RepositoryCommentReports.Search();
            commentReports = commentReports.Where(x => x.CommentId == info.CommentId && x.ReporterId == identity.Id);
            var commentReport = await commentReports.FirstOrDefaultAsync();

            // Post has been reported.
            if (commentReport != null)
                return StatusCode((int)HttpStatusCode.Conflict, new ApiResponse(HttpMessages.CommentHasBeenReported));

            #endregion

            #region Comment report initialization

            commentReport = new CommentReport();
            commentReport.CommentId = comment.Id;
            commentReport.PostId = comment.PostId;
            commentReport.OwnerId = post.OwnerId;
            commentReport.ReporterId = identity.Id;
            commentReport.Body = comment.Content;
            commentReport.Reason = info.Reason;
            commentReport.CreatedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Insert post to system.
            _unitOfWork.RepositoryCommentReports.Insert(commentReport);

            // Commit changes.
            await _unitOfWork.CommitAsync();

            #endregion

            return Ok(commentReport);
        }

        /// <summary>
        /// Find post report and edit it.
        /// </summary>
        /// <param name="commentId"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPut("")]
        public async Task<IActionResult> EditCommentReport([FromQuery] int commentId, [FromBody] EditCommentReportViewModel info)
        {
            #region Parameters validation

            if (info == null)
            {
                info = new EditCommentReportViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Comment report search

            // Find identity in request.
            var identity = _identityService.GetProfile(HttpContext);

            // Find comment reports.
            var commentReports = _unitOfWork.RepositoryCommentReports.Search();
            commentReports = commentReports.Where(x => x.CommentId == commentId && x.ReporterId == identity.Id);

            // Find the report.
            var commentReport = await commentReports.FirstOrDefaultAsync();
            if (commentReport == null)
                return NotFound(new ApiResponse(HttpMessages.CommentReportNotFound));

            #endregion

            #region Edit post report information

            // Update reason
            commentReport.Reason = info.Reason;
            commentReport.LastModifiedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Commit changes to system.
            await _unitOfWork.CommitAsync();

            #endregion

            return Ok(commentReport);
        }

        /// <summary>
        /// Delete a post report from system.
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        [HttpDelete("")]
        public async Task<IActionResult> DeleteCommentReport([FromQuery] int commentId)
        {
            #region Comment report search

            // Find identity in request.
            var identity = _identityService.GetProfile(HttpContext);

            // Find comment reports.
            var commentReports = _unitOfWork.RepositoryCommentReports.Search();
            commentReports = commentReports.Where(x => x.CommentId == commentId && x.ReporterId == identity.Id);

            // Find the report.
            var commentReport = await commentReports.FirstOrDefaultAsync();
            if (commentReport == null)
                return NotFound(new ApiResponse(HttpMessages.CommentReportNotFound));

            #endregion

            #region Update information

            // Update status to deleted.
            commentReport.Status = CommentReportStatus.Deleted;

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
        public async Task<IActionResult> SearchForPostReports([FromBody] SearchCommentReportViewModel condition)
        {
            #region Properties

            if (condition == null)
            {
                condition = new SearchCommentReportViewModel();
                TryValidateModel(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Search for post reports

            // Find identity from request.
            var identity = _identityService.GetProfile(HttpContext);

            // Get all post reports.
            var commentReports = _unitOfWork.RepositoryCommentReports.Search();

            // Comment id has been defined.
            if (condition.CommentId != null)
                commentReports = commentReports.Where(x => x.CommentId == condition.CommentId.Value);

            // Post id has been defined.
            if (condition.PostId != null)
                commentReports = commentReports.Where(x => x.PostId == condition.PostId.Value);

            // Post owner id has been defined.
            if (condition.OwnerId != null)
                commentReports = commentReports.Where(x => x.OwnerId == condition.OwnerId.Value);

            // Reason is defined.
            if (!string.IsNullOrWhiteSpace(condition.Reason))
                commentReports = commentReports.Where(x => x.Reason.Contains(condition.Reason));

            // Filter base on role.
            if (identity.Role == AccountRole.Admin)
            {
                // Reporter id has been defined.
                if (condition.ReporterId != null)
                    commentReports = commentReports.Where(x => x.ReporterId == condition.ReporterId.Value);

                // Statuses have been defined.
                if (condition.Statuses != null && condition.Statuses.Count > 0)
                    commentReports = commentReports.Where(x => condition.Statuses.Contains(x.Status));
            }
            else
            {
                commentReports = commentReports.Where(x => x.ReporterId == identity.Id);
                commentReports = commentReports.Where(x => x.Status == CommentReportStatus.Available);
            }

            // Created time has been defined.
            var createdTime = condition.CreatedTime;
            if (createdTime != null)
            {
                var from = createdTime.From;
                var to = createdTime.To;

                if (from != null)
                    commentReports = _unitOfWork.RepositoryCommentReports.SearchNumericProperty(commentReports,
                        x => x.CreatedTime, from.Value, NumericComparision.GreaterEqual);

                if (to != null)
                    commentReports = _unitOfWork.RepositoryCommentReports.SearchNumericProperty(commentReports,
                        x => x.CreatedTime, to.Value, NumericComparision.LowerEqual);
            }

            // Last modified time has been defined.
            var lastModifiedTime = condition.LastModifiedTime;
            if (lastModifiedTime != null)
            {
                var from = lastModifiedTime.From;
                var to = lastModifiedTime.To;

                if (from != null)
                    commentReports = _unitOfWork.RepositoryCommentReports.SearchNumericProperty(commentReports,
                        x => x.LastModifiedTime, from.Value, NumericComparision.GreaterEqual);

                if (to != null)
                    commentReports = _unitOfWork.RepositoryCommentReports.SearchNumericProperty(commentReports,
                        x => x.LastModifiedTime, to.Value, NumericComparision.LowerEqual);
            }

            // Sorting.
            var sort = condition.Sort;
            if (sort != null)
                commentReports = _unitOfWork.RepositoryCommentReports.Sort(commentReports, sort.Direction, sort.Property);
            else
                commentReports = _unitOfWork.RepositoryCommentReports.Sort(commentReports, SortDirection.Decending,
                    PostReportSort.CreatedTime);

            #endregion

            #region Result search and count

            // Count post task initialization.
            var pCountCommentReports = commentReports.CountAsync();
            var pGetCommentReports = _unitOfWork.Paginate(commentReports, condition.Pagination).ToListAsync();

            // Wait for all tasks to complete.
            await Task.WhenAll(pCountCommentReports, pGetCommentReports);

            // Result initialization.
            var result = new SearchResult<IList<CommentReport>>();
            result.Records = pGetCommentReports.Result;
            result.Total = pCountCommentReports.Result;

            #endregion

            return Ok(result);
        }
        
        #endregion
    }
}