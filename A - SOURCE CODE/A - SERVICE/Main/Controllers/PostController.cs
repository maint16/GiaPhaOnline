using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemConstant.Enumerations;
using SystemConstant.Enumerations.Order;
using SystemConstant.Models;
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
using Shared.ViewModels.FollowPosts;
using Shared.ViewModels.Posts;

namespace Main.Controllers
{
    [Route("api/[controller]")]
    public class PostController : Controller
    {
        #region Properties

        /// <summary>
        /// Instance which is for accessing to database.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Instance which is for accessing automapper.
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Instance which is for accessing identity attached in request.
        /// </summary>
        private readonly IIdentityService _identityService;

        /// <summary>
        /// Service which is for time calculation.
        /// </summary>
        private readonly ITimeService _timeService;

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
        /// <param name="identityService"></param>
        /// <param name="timeService"></param>
        /// <param name="databaseFunction"></param>
        public PostController(IUnitOfWork unitOfWork, IMapper mapper, IIdentityService identityService, ITimeService timeService, IDbSharedService databaseFunction)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _identityService = identityService;
            _timeService = timeService;
            _databaseFunction = databaseFunction;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add a post into system.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<IActionResult> AddPost([FromBody] AddPostViewModel info)
        {
            #region Parameters validation

            // Information is invalid.
            if (info == null)
            {
                info = new AddPostViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Post initialization

            // Find request identity.
            var identity = _identityService.GetProfile(HttpContext);

            // Initialize post.
            var post = _mapper.Map<AddPostViewModel, Post>(info);
            post.Status = PostStatus.Available;
            post.OwnerId = identity.Id;
            post.CreatedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Add entity into system.
            _unitOfWork.Posts.Insert(post);

            // Commit changes.
            await _unitOfWork.CommitAsync();

            #endregion

            return Ok(post);
        }

        /// <summary>
        /// Edit post with specific information.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditPost([FromRoute] int id, [FromBody] EditPostViewModel info)
        {
            #region Parameters validation

            if (info == null)
            {
                info = new EditPostViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Search for post

            // Find identity in request.
            var identity = _identityService.GetProfile(HttpContext);

            var posts = _unitOfWork.Posts.Search();
            posts = posts.Where(x => x.Id == id && x.Status == PostStatus.Available && x.OwnerId == identity.Id);

            // Find post from list.
            var post = await posts.FirstOrDefaultAsync();
            if (post == null)
                return NotFound(new ApiResponse(HttpMessages.PostNotFound));

            #endregion

            #region Information update

            // Whether information has been changed or not.
            var bHasInformationChanged = false;

            // Title has been defined.
            if (!string.IsNullOrWhiteSpace(info.Title))
            {
                post.Title = info.Title;
                bHasInformationChanged = true;
            }

            // Body has been defined.
            if (!string.IsNullOrWhiteSpace(info.Body))
            {
                post.Body = info.Body;
                bHasInformationChanged = true;
            }

            // Status has been defined.
            if (info.Type != null)
            {
                post.Type = info.Type.Value;
                bHasInformationChanged = true;
            }

            // Information has been changed.
            if (bHasInformationChanged)
                await _unitOfWork.CommitAsync();

            #endregion

            return Ok(post);
        }

        /// <summary>
        /// Delete a specific post.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost([FromRoute] int id)
        {
            // Find post by using post.
            var posts = _unitOfWork.Posts.Search();

            // Find requester identity.
            var identity = _identityService.GetProfile(HttpContext);

            // Find post by id.
            posts = posts.Where(x => x.Id == id && x.Status == PostStatus.Available);

            // User can only delete his/her own posts. Admin can delete all.
            if (identity.Role != AccountRole.Admin)
                posts = posts.Where(x => x.OwnerId == identity.Id);

            // Find the first match record.
            var post = await posts.FirstOrDefaultAsync();

            // Post is not found.
            if (post == null)
                return NotFound(new ApiResponse(HttpMessages.PostNotFound));

            return Ok();
        }

        /// <summary>
        /// Search for posts by using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("search")]
        [ByPassAuthorization]
        public async Task<IActionResult> SearchForPosts([FromBody] SearchPostViewModel condition)
        {
            #region Parameters validation

            if (condition == null)
            {
                condition = new SearchPostViewModel();
                TryValidateModel(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Search for information

            // Find identity in request.
            var identity = _identityService.GetProfile(HttpContext);

            // Search for posts.
            var posts = _unitOfWork.Posts.Search();

            // Id is defined.
            if (condition.Id != null)
                posts = posts.Where(x => x.Id == condition.Id);

            // Owner is defined.
            if (condition.OwnerId != null)
                posts = posts.Where(x => x.OwnerId == condition.OwnerId);

            // Title search condition has been defined.
            if (condition.Title != null && !string.IsNullOrWhiteSpace(condition.Title))
                posts = _databaseFunction.SearchPropertyText(posts, x => x.Title,
                    new TextSearch(TextSearchMode.ContainIgnoreCase, condition.Title));

            // Search conditions which are based on roles.
            if (identity != null)
            {
                if (identity.Role == AccountRole.Admin)
                {
                    // Statuses are defined.
                    if (condition.Statuses != null && condition.Statuses.Count > 0)
                        posts = posts.Where(x => condition.Statuses.Contains(x.Status));

                    // Types are defined. Only admin can see post types.
                    if (condition.Types != null && condition.Types.Count > 0)
                        posts = posts.Where(x => condition.Types.Contains(x.Type));
                }
                else
                {
                    // Normal users can only see available posts or their own posts.
                    posts = posts.Where(x =>
                        x.Status == PostStatus.Available || (x.OwnerId == identity.Id && x.Status != PostStatus.Available));

                    // Normal users can see public posts.
                    posts = posts.Where(x => x.Type == PostType.Public);
                }
            }
            else
            {
                // Normal users can only see avaiable posts
                posts = posts.Where(x =>
                    x.Status == PostStatus.Available);

                // Normal users can see public posts
                posts = posts.Where(x => x.Type == PostType.Public);
            }
            

            // Created time has been defined.
            var createdTime = condition.CreatedTime;
            if (createdTime != null)
            {
                var from = createdTime.From;
                var to = createdTime.To;

                if (from != null)
                    posts = _databaseFunction.SearchNumericProperty(posts, x => x.CreatedTime, from.Value,
                        NumericComparision.GreaterEqual);

                if (to != null)
                    posts = _databaseFunction.SearchNumericProperty(posts, x => x.CreatedTime, to.Value,
                        NumericComparision.LowerEqual);
            }

            // Created time has been defined.
            var lastModifiedTime = condition.LastModifiedTime;
            if (lastModifiedTime != null)
            {
                var from = lastModifiedTime.From;
                var to = lastModifiedTime.To;

                if (from != null)
                    posts = _databaseFunction.SearchNumericProperty(posts, x => x.LastModifiedTime, from.Value,
                        NumericComparision.GreaterEqual);

                if (to != null)
                    posts = _databaseFunction.SearchNumericProperty(posts, x => x.LastModifiedTime, to.Value,
                        NumericComparision.LowerEqual);
            }

            // Sort property & direction.
            var sort = condition.Sort;
            if (sort != null)
                posts = _databaseFunction.Sort(posts, sort.Direction, sort.Property);
            else
                posts = _databaseFunction.Sort(posts, SortDirection.Decending, PostSort.CreatedTime);

            var result = new SearchResult<IList<Post>>();

            result.Total = await posts.CountAsync();
            result.Records = await _databaseFunction.Paginate(posts, condition.Pagination).ToListAsync();

            #endregion

            return Ok(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("load-posts")]
        public async Task<IActionResult> LoadPosts([FromBody] LoadPostViewModel condition)
        {
            #region Parameters validation

            if (condition == null)
            {
                condition = new LoadPostViewModel();
                TryValidateModel(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Search for information

            // Get all posts
            var posts = _unitOfWork.Posts.Search();
            posts = LoadPosts(posts, condition);

            // Sort by properties.
            if (condition.Sort != null)
                posts =
                    _databaseFunction.Sort(posts, condition.Sort.Direction,
                        condition.Sort.Property);
            else
                posts = _databaseFunction.Sort(posts, SortDirection.Decending,
                    PostSort.CreatedTime);

            // Result initialization.
            var result = new SearchResult<IList<Post>>();
            result.Total = await posts.CountAsync();
            result.Records = await _databaseFunction.Paginate(posts, condition.Pagination).ToListAsync();

            #endregion

            return Ok(result);
        }

        /// <summary>
        ///     Load posts by using specific conditions.
        /// </summary>
        /// <param name="posts"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public IQueryable<Post> LoadPosts(IQueryable<Post> posts,
            LoadPostViewModel conditions)
        {
            if (conditions == null)
                return posts;

            // Id has been defined.
            if (conditions.Ids != null && conditions.Ids.Count > 0)
            {
                conditions.Ids = conditions.Ids.Where(x => x > 0).ToList();
                if (conditions.Ids.Count > 0)
                    posts = posts.Where(x => conditions.Ids.Contains(x.Id));
            }

            return posts;
        }

        /// <summary>
        /// Change post status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPut("status/{id}")]
        [Authorize(Policy = PolicyConstant.IsAdminPolicy)]
        public async Task<IActionResult> ChangePostStatus([FromRoute] int id, [FromBody] ChangePostStatusViewModel info)
        {
            #region Parameters validation

            if (info == null)
            {
                info = new ChangePostStatusViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Search for post

            // Find identity in request.
            var identity = _identityService.GetProfile(HttpContext);

            var posts = _unitOfWork.Posts.Search();
            posts = posts.Where(x => x.Id == id);

            // Find post from list.
            var post = await posts.FirstOrDefaultAsync();
            if (post == null)
                return NotFound(new ApiResponse(HttpMessages.PostNotFound));

            #endregion

            #region Information update

            // Whether information has been changed or not.
            var bHasInformationChanged = false;

            // Status has been defined.
            if (info.Status != post.Status)
            {
                post.Status = info.Status;
                bHasInformationChanged = true;
            }

            //todo: Reason

            // Information has been changed.
            if (bHasInformationChanged)
                await _unitOfWork.CommitAsync();

            #endregion

            return Ok(post);
        }

        #endregion
    }
}