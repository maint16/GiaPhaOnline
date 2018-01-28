using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemConstant.Enumerations;
using SystemDatabase.Interfaces;
using SystemDatabase.Models.Entities;
using Main.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Services;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels;
using Shared.ViewModels.Comments;

namespace Main.Controllers
{
    [Route("api/[comment]")]
    public class CommentController : Controller
    {
        #region Constructors

        /// <summary>
        ///     Initialize controller with injectors.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="identityService"></param>
        /// <param name="timeService"></param>
        public CommentController(IUnitOfWork unitOfWork, IIdentityService identityService, ITimeService timeService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _timeService = timeService;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Add comment to system.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<IActionResult> AddComment([FromBody] AddCommentViewModel info)
        {
            #region Parameters validation

            if (info == null)
            {
                info = new AddCommentViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Find post

            // Find posts.
            var posts = _unitOfWork.RepositoryPosts.Search();
            posts = posts.Where(x => x.Id == info.PostId && x.Status == PostStatus.Available);

            // Check whether post exists or not.
            var bIsPostAvailable = await posts.AnyAsync();
            if (!bIsPostAvailable)
                return NotFound(new ApiResponse(HttpMessages.PostCategorizationNotFound));

            #endregion

            #region Comment initialization

            // Find identity from request.
            var identity = _identityService.GetProfile(HttpContext);

            // Comment intialization.
            var comment = new Comment();
            comment.OwnerId = identity.Id;
            comment.PostId = info.PostId;
            comment.Status = CommentStatus.Available;
            comment.Content = info.Content;
            comment.CreatedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Insert comment into system.
            _unitOfWork.RepositoryComments.Insert(comment);

            await _unitOfWork.CommitAsync();

            #endregion

            return Ok(comment);
        }

        /// <summary>
        /// Edit comment by using specific information.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditComment([FromQuery] int id, [FromBody] EditCommentViewModel info)
        {
            #region Parameters validation

            if (info == null)
            {
                info = new EditCommentViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Find comment

            // Get request identity.
            var identity = _identityService.GetProfile(HttpContext);

            // Get all comments in database.
            var comments = _unitOfWork.RepositoryComments.Search();
            comments = comments.Where(x => x.Id == id && x.OwnerId == identity.Id && x.Status == CommentStatus.Available);

            // Get the first matched comments.
            var comment = await comments.FirstOrDefaultAsync();
            if (comment == null)
                return NotFound(new ApiResponse(HttpMessages.CommentNotFound));

            #endregion
            
            #region Update comment information

            // Update content.
            comment.Content = info.Content;
            comment.LastModifiedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Commit changes to database.
            await _unitOfWork.CommitAsync();

            #endregion

            return Ok(comment);
        }

        /// <summary>
        /// Delete a comment by using specific information.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment([FromQuery] int id)
        {
            #region Find comment

            // Get request identity.
            var identity = _identityService.GetProfile(HttpContext);

            // Get all comments in database.
            var comments = _unitOfWork.RepositoryComments.Search();
            comments = comments.Where(x => x.Id == id && x.OwnerId == identity.Id && x.Status == CommentStatus.Available);

            // Get the first matched comments.
            var comment = await comments.FirstOrDefaultAsync();
            if (comment == null)
                return NotFound(new ApiResponse(HttpMessages.CommentNotFound));

            #endregion

            #region Change comment information

            // Change comment status.
            comment.Status = CommentStatus.Deleted;

            // Commit to system.
            await _unitOfWork.CommitAsync();

            #endregion

            return Ok();
        }

        /// <summary>
        /// Search for comments using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchComments([FromBody] SearchCommentViewModel condition)
        {
            #region Parameters validation

            if (condition == null)
            {
                condition = new SearchCommentViewModel();
                TryValidateModel(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Search for information

            // Get request identity.
            var identity = _identityService.GetProfile(HttpContext);

            // Search for all comments.
            var comments = _unitOfWork.RepositoryComments.Search();

            // Id has been defined.
            if (condition.Id != null)
                comments = comments.Where(x => x.Id == condition.Id.Value);

            // Search by owner index (Admin is supported only)
            if (condition.OwnerId != null && identity.Role == AccountRole.Admin)
                comments = comments.Where(x => x.OwnerId == condition.OwnerId.Value);

            // Search by post id.
            if (condition.PostId != null)
                comments = comments.Where(x => x.PostId == condition.PostId.Value);

            // Content is defined.
            if (!string.IsNullOrWhiteSpace(condition.Content))
                comments = comments.Where(x => x.Content.Contains(condition.Content));

            // Created time is defined.
            var createdTime = condition.CreatedTime;
            if (createdTime != null)
            {
                var from = createdTime.From;
                var to = createdTime.To;

                if (from != null)
                    comments = _unitOfWork.RepositoryComments.SearchNumericProperty(comments, x => x.CreatedTime,
                        from.Value, NumericComparision.GreaterEqual);
                if (to != null)
                    comments = _unitOfWork.RepositoryComments.SearchNumericProperty(comments, x => x.CreatedTime,
                        to.Value, NumericComparision.LowerEqual);
            }

            // Last modified time is defined.
            var lastModifiedTime = condition.LastModifiedTime;
            if (lastModifiedTime != null)
            {
                var from = lastModifiedTime.From;
                var to = lastModifiedTime.To;

                if (from != null)
                    comments = _unitOfWork.RepositoryComments.SearchNumericProperty(comments, x => x.LastModifiedTime,
                        from.Value, NumericComparision.GreaterEqual);
                if (to != null)
                    comments = _unitOfWork.RepositoryComments.SearchNumericProperty(comments, x => x.LastModifiedTime,
                        to.Value, NumericComparision.LowerEqual);
            }

            #endregion

            #region Count and paging

            var result = new SearchResult<IList<Comment>>();

            // Count comments.
            var pCountCommentTask = comments.CountAsync();
            
            // Paginate comments.
            var pGetCommentTask = _unitOfWork.Paginate(comments, condition.Pagination).ToListAsync();

            // Wait for all tasks to complete.
            await Task.WhenAll(pGetCommentTask, pGetCommentTask);
            result.Records = pGetCommentTask.Result;
            result.Total = pCountCommentTask.Result;

            #endregion

            return Ok(result);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Instance which for accessing database.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        ///     Instance which is for accessing identity attached into request.
        /// </summary>
        private readonly IIdentityService _identityService;

        /// <summary>
        ///     Instance which is for calculating time.
        /// </summary>
        private readonly ITimeService _timeService;

        #endregion
    }
}