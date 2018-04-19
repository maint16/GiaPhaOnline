using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SystemConstant.Enumerations;
using SystemConstant.Enumerations.Order;
using SystemDatabase.Interfaces;
using SystemDatabase.Interfaces.Repositories;
using SystemDatabase.Models.Entities;
using AutoMapper;
using Main.Authentications.ActionFilters;
using Main.Constants;
using Main.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Services;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels;
using Shared.ViewModels.Comments;
using Shared.ViewModels.Posts;

namespace Main.Controllers
{
    [Route("api/[controller]")]
    public class CommentController : ApiBaseController
    {
        #region Properties

        /// <summary>
        /// Instance which is for accessing to database.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Service which is for accessing database function.
        /// </summary>
        private readonly IDbSharedService _databaseFunction;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize controller with injectors.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="mapper"></param>
        /// <param name="timeService"></param>
        /// <param name="dbSharedService"></param>
        /// <param name="identityService"></param>
        public CommentController(IUnitOfWork unitOfWork, IMapper mapper, ITimeService timeService, IDbSharedService dbSharedService, IIdentityService identityService) : base(unitOfWork, mapper, timeService, dbSharedService, identityService)
        {
            _unitOfWork = unitOfWork;
            _databaseFunction = dbSharedService;
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
            var posts = UnitOfWork.Posts.Search();
            posts = posts.Where(x => x.Id == info.PostId && x.Status == PostStatus.Available);

            // Check whether post exists or not.
            var bIsPostAvailable = await posts.AnyAsync();
            if (!bIsPostAvailable)
                return NotFound(new ApiResponse(HttpMessages.PostNotFound));

            #endregion

            #region Comment initialization

            // Find identity from request.
            var identity = IdentityService.GetProfile(HttpContext);

            // Comment intialization.
            var comment = new Comment();
            comment.OwnerId = identity.Id;
            comment.PostId = info.PostId;
            comment.Status = ItemStatus.Available;
            comment.Content = info.Content;
            comment.CreatedTime = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Insert comment into system.
            UnitOfWork.Comments.Insert(comment);

            await UnitOfWork.CommitAsync();

            #endregion

            return Ok(comment);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost("load-comments")]
        public async Task<IActionResult> GetComments([FromBody] [Required] HashSet<int> ids )
        {
            var result = new SearchResult<IList<Comment>>();

            // Get request identity.
            var identity = IdentityService.GetProfile(HttpContext);

            // Indexes filter.
            ids = ids.Where(x => x > 0).ToHashSet();

            // Get comments from database.
            var comments = UnitOfWork.Comments.Search();

            if (identity.Role == AccountRole.User)
                comments = comments.Where(x => x.Status == ItemStatus.Available);

            comments = comments.Where(x => ids.Contains(x.Id));

            result.Records = await comments.ToListAsync();
            result.Total = result.Records.Count;

            return Ok(result);
        }

        /// <summary>
        /// Edit comment by using specific information.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditComment([FromRoute] int id, [FromBody] EditCommentViewModel info)
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
            var identity = IdentityService.GetProfile(HttpContext);

            // Get all comments in database.
            var comments = UnitOfWork.Comments.Search();
            comments = comments.Where(x => x.Id == id && x.OwnerId == identity.Id && x.Status == ItemStatus.Available);

            // Get the first matched comments.
            var comment = await comments.FirstOrDefaultAsync();
            if (comment == null)
                return NotFound(new ApiResponse(HttpMessages.CommentNotFound));

            #endregion

            #region Update comment information

            // Update content.
            comment.Content = info.Content;
            comment.LastModifiedTime = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Commit changes to database.
            await UnitOfWork.CommitAsync();

            #endregion

            return Ok(comment);
        }

        /// <summary>
        /// Delete a comment by using specific information.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment([FromRoute] int id)
        {
            #region Find comment

            // Get request identity.
            var identity = IdentityService.GetProfile(HttpContext);

            // Get all comments in database.
            var comments = UnitOfWork.Comments.Search();
            comments = comments.Where(x => x.Id == id && x.OwnerId == identity.Id && x.Status == ItemStatus.Available);

            // Get the first matched comments.
            var comment = await comments.FirstOrDefaultAsync();
            if (comment == null)
                return NotFound(new ApiResponse(HttpMessages.CommentNotFound));

            #endregion

            #region Change comment information

            // Change comment status.
            comment.Status = ItemStatus.NotAvailable;

            // Commit to system.
            await UnitOfWork.CommitAsync();

            #endregion

            return Ok();
        }

        /// <summary>
        /// Search for comments using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("search")]
        [ByPassAuthorization]
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

            // Get all comments
            var comments = UnitOfWork.Comments.Search();
            comments = SearchComments(comments, condition);

            #endregion

            #region Count and paging

            // Sort by properties.
            if (condition.Sort != null)
                comments =
                    DbSharedService.Sort(comments, condition.Sort.Direction,
                        condition.Sort.Property);
            else
                comments = DbSharedService.Sort(comments, SortDirection.Decending,
                    CommentSort.CreatedTime);

            var result = new SearchResult<IList<Comment>>();
            result.Total = await comments.CountAsync();
            result.Records = await DbSharedService.Paginate(comments, condition.Pagination).ToListAsync();

            #endregion

            return Ok(result);
        }
        
        /// <summary>
        /// Search for comments by using specific conditions.
        /// </summary>
        /// <param name="comments"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        private IQueryable<Comment> SearchComments(IQueryable<Comment> comments, SearchCommentViewModel condition)
        {
            #region Search for information

            // Get request identity.
            var identity = IdentityService.GetProfile(HttpContext);

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
                    comments = DbSharedService.SearchNumericProperty(comments, x => x.CreatedTime,
                        from.Value, NumericComparision.GreaterEqual);
                if (to != null)
                    comments = DbSharedService.SearchNumericProperty(comments, x => x.CreatedTime,
                        to.Value, NumericComparision.LowerEqual);
            }

            // Last modified time is defined.
            var lastModifiedTime = condition.LastModifiedTime;
            if (lastModifiedTime != null)
            {
                var from = lastModifiedTime.From;
                var to = lastModifiedTime.To;

                if (from != null)
                    comments = DbSharedService.SearchNumericProperty(comments, x => x.LastModifiedTime,
                        from.Value, NumericComparision.GreaterEqual);
                if (to != null)
                    comments = DbSharedService.SearchNumericProperty(comments, x => x.LastModifiedTime,
                        to.Value, NumericComparision.LowerEqual);
            }

            #endregion

            return comments;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("load-comments")]
        public async Task<IActionResult> LoadComments([FromBody] LoadCommentViewModel condition)
        {
            #region Parameters validation

            if (condition == null)
            {
                condition = new LoadCommentViewModel();
                TryValidateModel(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Search for information

            // Get all comments
            var comments = _unitOfWork.Comments.Search();
            comments = LoadComments(comments, condition);

            // Sort by properties.
            if (condition.Sort != null)
                comments =
                    _databaseFunction.Sort(comments, condition.Sort.Direction,
                        condition.Sort.Property);
            else
                comments = _databaseFunction.Sort(comments, SortDirection.Decending,
                    CommentSort.CreatedTime);

            // Result initialization.
            var result = new SearchResult<IList<Comment>>();
            result.Total = await comments.CountAsync();
            result.Records = await _databaseFunction.Paginate(comments, condition.Pagination).ToListAsync();

            #endregion

            return Ok(result);
        }

        /// <summary>
        ///     Load comments by using specific conditions.
        /// </summary>
        /// <param name="comments"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public IQueryable<Comment> LoadComments(IQueryable<Comment> comments,
            LoadCommentViewModel conditions)
        {
            if (conditions == null)
                return comments;

            // Id has been defined.
            if (conditions.Ids != null && conditions.Ids.Count > 0)
            {
                conditions.Ids = conditions.Ids.Where(x => x > 0).ToList();
                if (conditions.Ids.Count > 0)
                    comments = comments.Where(x => conditions.Ids.Contains(x.Id));
            }

            return comments;
        }

        /// <summary>
        /// Change comment status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPut("status/{id}")]
        [Authorize(Policy = PolicyConstant.IsAdminPolicy)]
        public async Task<IActionResult> ChangeCommentStatus([FromRoute] int id, [FromBody] ChangeCommentStatusViewModel info)
        {
            #region Parameters validation

            if (info == null)
            {
                info = new ChangeCommentStatusViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Search for post

            var comments = _unitOfWork.Comments.Search();
            comments = comments.Where(x => x.Id == id);

            // Find comment from list.
            var comment = await comments.FirstOrDefaultAsync();
            if (comment == null)
                return NotFound(new ApiResponse(HttpMessages.CommentNotFound));

            #endregion

            #region Information update

            // Whether information has been changed or not.
            var bHasInformationChanged = false;

            // Status has been defined.
            if (info.Status != comment.Status)
            {
                comment.Status = info.Status;
                bHasInformationChanged = true;
            }

            //todo: Reason

            // Information has been changed.
            if (bHasInformationChanged)
                await _unitOfWork.CommitAsync();

            #endregion

            return Ok(comment);
        }

        #endregion

    }
}