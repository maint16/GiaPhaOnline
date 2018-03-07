using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SystemConstant.Enumerations;
using SystemConstant.Enumerations.Order;
using SystemDatabase.Interfaces;
using SystemDatabase.Interfaces.Repositories;
using SystemDatabase.Models.Entities;
using AutoMapper;
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
    public class CommentReportController : ApiBaseController
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
        public CommentReportController(IUnitOfWork unitOfWork, IMapper mapper, ITimeService timeService, IDbSharedService dbSharedService, IIdentityService identityService) : base(unitOfWork, mapper, timeService, dbSharedService, identityService)
        {
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
            var identity = IdentityService.GetProfile(HttpContext);

            // Find comments.
            var comments = UnitOfWork.Comments.Search();
            comments = comments.Where(x =>
                x.Id == info.CommentId && x.OwnerId != identity.Id && x.Status == ItemStatus.Available);

            // Find comment which matches to the search condition.
            var comment = await comments.FirstOrDefaultAsync();
            if (comment == null)
                return NotFound(new ApiResponse(HttpMessages.CommentNotFound));

            #endregion

            #region Check post status

            // Search for posts.
            var posts = UnitOfWork.Posts.Search();
            posts = posts.Where(x => x.Id == comment.PostId && x.Status == PostStatus.Available);
            var post = await posts.FirstOrDefaultAsync();
            if (post == null)
                return NotFound(new ApiResponse(HttpMessages.PostNotFound));

            #endregion

            #region Check report duplicate

            // Find comment reports.
            var commentReports = UnitOfWork.CommentReports.Search();
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
            commentReport.CreatedTime = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Insert post to system.
            UnitOfWork.CommentReports.Insert(commentReport);

            // Commit changes.
            await UnitOfWork.CommitAsync();

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
        public async Task<IActionResult> EditCommentReport([FromRoute] int commentId, [FromBody] EditCommentReportViewModel info)
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
            var identity = IdentityService.GetProfile(HttpContext);

            // Find comment reports.
            var commentReports = UnitOfWork.CommentReports.Search();
            commentReports = commentReports.Where(x => x.CommentId == commentId && x.ReporterId == identity.Id);

            // Find the report.
            var commentReport = await commentReports.FirstOrDefaultAsync();
            if (commentReport == null)
                return NotFound(new ApiResponse(HttpMessages.CommentReportNotFound));

            #endregion

            #region Edit post report information

            // Update reason
            commentReport.Reason = info.Reason;
            commentReport.LastModifiedTime = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Commit changes to system.
            await UnitOfWork.CommitAsync();

            #endregion

            return Ok(commentReport);
        }

        /// <summary>
        /// Delete a post report from system.
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        [HttpDelete("")]
        public async Task<IActionResult> DeleteCommentReport([FromRoute] int commentId)
        {
            #region Comment report search

            // Find identity in request.
            var identity = IdentityService.GetProfile(HttpContext);

            // Find comment reports.
            var commentReports = UnitOfWork.CommentReports.Search();
            commentReports = commentReports.Where(x => x.CommentId == commentId && x.ReporterId == identity.Id);

            // Find the report.
            var commentReport = await commentReports.FirstOrDefaultAsync();
            if (commentReport == null)
                return NotFound(new ApiResponse(HttpMessages.CommentReportNotFound));

            #endregion

            #region Update information

            // Update status to deleted.
            commentReport.Status = ItemStatus.NotAvailable;

            // Commit data.
            await UnitOfWork.CommitAsync();

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
            var identity = IdentityService.GetProfile(HttpContext);

            // Get all post reports.
            var commentReports = UnitOfWork.CommentReports.Search();

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
                commentReports = commentReports.Where(x => x.Status == ItemStatus.Available);
            }

            // Created time has been defined.
            var createdTime = condition.CreatedTime;
            if (createdTime != null)
            {
                var from = createdTime.From;
                var to = createdTime.To;

                if (from != null)
                    commentReports = DbSharedService.SearchNumericProperty(commentReports,
                        x => x.CreatedTime, from.Value, NumericComparision.GreaterEqual);

                if (to != null)
                    commentReports = DbSharedService.SearchNumericProperty(commentReports,
                        x => x.CreatedTime, to.Value, NumericComparision.LowerEqual);
            }

            // Last modified time has been defined.
            var lastModifiedTime = condition.LastModifiedTime;
            if (lastModifiedTime != null)
            {
                var from = lastModifiedTime.From;
                var to = lastModifiedTime.To;

                if (from != null)
                    commentReports = DbSharedService.SearchNumericProperty(commentReports,
                        x => x.LastModifiedTime, from.Value, NumericComparision.GreaterEqual);

                if (to != null)
                    commentReports = DbSharedService.SearchNumericProperty(commentReports,
                        x => x.LastModifiedTime, to.Value, NumericComparision.LowerEqual);
            }

            // Sorting.
            var sort = condition.Sort;
            if (sort != null)
                commentReports = DbSharedService.Sort(commentReports, sort.Direction, sort.Property);
            else
                commentReports = DbSharedService.Sort(commentReports, SortDirection.Decending,
                    PostReportSort.CreatedTime);

            #endregion

            #region Result search and count
            
            // Result initialization.
            var result = new SearchResult<IList<CommentReport>>();
            
            result.Total = await commentReports.CountAsync();
            result.Records = await DbSharedService.Paginate(commentReports, condition.Pagination).ToListAsync();;

            #endregion

            return Ok(result);
        }
        
        #endregion
        
    }
}