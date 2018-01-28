using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SystemConstant.Enumerations;
using SystemConstant.Enumerations.Order;
using SystemConstant.Models;
using SystemDatabase.Interfaces;
using SystemDatabase.Models.Entities;
using AutoMapper;
using Main.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Services;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels;
using Shared.ViewModels.Categories;
using Shared.ViewModels.FollowPosts;
using SkiaSharp;

namespace Main.Controllers
{
    public class FollowPostController : Controller
    {
        #region Constructors

        /// <summary>
        ///     Initialize controller with injectors.
        /// </summary>
        /// <param name="identityService">Service which is for handling identity.</param>
        /// <param name="timeService">Service which is for handling time calculation.</param>
        /// <param name="unitOfWork">Instance for accessing database.</param>
        /// <param name="mapper">Instance for mapping objects</param>
        public FollowPostController(IIdentityService identityService, ITimeService timeService, IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _identityService = identityService;
            _timeService = timeService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Service which is for handling identity.
        /// </summary>
        private readonly IIdentityService _identityService;

        /// <summary>
        ///     Service which is for handling time calculation.
        /// </summary>
        private readonly ITimeService _timeService;

        /// <summary>
        ///     Instance for accessing database.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        ///     Instance for mapping objects.
        /// </summary>
        private readonly IMapper _mapper;

        #endregion

        #region Methods

        /// <summary>
        ///     Add a category into database.
        /// </summary>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<IActionResult> StartFollowingPost([FromQuery] int postId)
        {
            #region Find post

            // Get posts by using id.
            var posts = _unitOfWork.RepositoryPosts.Search();
            posts = posts.Where(x => x.Id == postId && x.Status == PostStatus.Available);
            var post = await posts.FirstOrDefaultAsync();

            // Post is not found.
            if (post == null)
                return NotFound(new ApiResponse(HttpMessages.PostNotFound));

            #endregion

            #region Check following duplicate

            // Get identity from request.
            var identity = _identityService.GetProfile(HttpContext);

            // Get follow posts.
            var followPosts = _unitOfWork.RepositoryFollowPosts.Search();
            followPosts = followPosts.Where(x => x.FollowerId == identity.Id && x.PostId == post.Id);
            var followPost = await followPosts.FirstOrDefaultAsync();

            if (followPost != null)
                followPost.Status = FollowPostStatus.Available;
            else
            {
                followPost = new FollowPost();
                followPost.FollowerId = identity.Id;
                followPost.PostId = postId;
                followPost.Status = FollowPostStatus.Available;
                followPost.CreatedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

                // Insert record into system.
                _unitOfWork.RepositoryFollowPosts.Insert(followPost);
            }

            // Commit changes to system.
            await _unitOfWork.CommitAsync();

            #endregion

            return Ok(followPost);
        }

        /// <summary>
        /// Stop following a specific post.
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> StopFollowingPost([FromQuery] int postId)
        {
            #region Check following duplicate

            // Get identity from request.
            var identity = _identityService.GetProfile(HttpContext);

            // Get follow posts.
            var followPosts = _unitOfWork.RepositoryFollowPosts.Search();
            followPosts = followPosts.Where(x => x.FollowerId == identity.Id && x.PostId == postId);
            var followPost = await followPosts.FirstOrDefaultAsync();

            if (followPost == null)
                return NotFound(new ApiResponse(HttpMessages.PostHasntBeenFollowedYet));

            // Update follow post.
            followPost.Status = FollowPostStatus.Deleted;

            // Commit changes to system.
            await _unitOfWork.CommitAsync();
            
            #endregion

            return Ok(followPost);
        }

        /// <summary>
        ///     Search for a list of categories.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchFollowingPosts([FromBody] SearchFollowPostViewModel condition)
        {
            #region Parameters validation

            if (condition == null)
            {
                condition = new SearchFollowPostViewModel();
                TryValidateModel(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Search for information

            // Find request identity.
            var identity = _identityService.GetProfile(HttpContext);

            // Get all categories.
            var followPosts = _unitOfWork.RepositoryFollowPosts.Search();
            
            // Post id is defined.
            if (condition.PostId != null)
                followPosts = followPosts.Where(x => x.PostId == condition.PostId.Value);

            // Statuses are defined.
            if (condition.Statuses != null && condition.Statuses.Count > 0)
            {
                condition.Statuses = condition.Statuses.Where(x => Enum.IsDefined(typeof(FollowPostStatus), x))
                    .ToHashSet();

                if (condition.Statuses.Count > 0)
                    followPosts = followPosts.Where(x => condition.Statuses.Contains(x.Status));
            }

            // Search by role.
            if (identity.Role == AccountRole.Admin)
            {
                // Admin can search for followers.
                if (condition.FollowerId != null)
                    followPosts = followPosts.Where(x => x.FollowerId == condition.FollowerId.Value);
            }
            else
            {
                followPosts = followPosts.Where(x => x.FollowerId == identity.Id);
            }

            // Created time is defined.
            var createdTime = condition.CreatedTime;
            if (createdTime != null)
            {
                var from = createdTime.From;
                var to = createdTime.To;

                if (from != null)
                    followPosts = _unitOfWork.RepositoryFollowPosts.SearchNumericProperty(followPosts,
                        x => x.CreatedTime, from.Value, NumericComparision.GreaterEqual);

                if (to != null)
                    followPosts = _unitOfWork.RepositoryFollowPosts.SearchNumericProperty(followPosts,
                        x => x.CreatedTime, to.Value, NumericComparision.LowerEqual);
            }

            // Sort by properties.
            if (condition.Sort != null)
                followPosts =
                    _unitOfWork.RepositoryFollowPosts.Sort(followPosts, condition.Sort.Direction,
                        condition.Sort.Property);
            else
                followPosts = _unitOfWork.RepositoryFollowPosts.Sort(followPosts, SortDirection.Decending,
                    CategoriesSort.CreatedTime);

            #endregion

            #region Result gathering

            // Tasks initialization.
            var pGetFollowPostsCount = followPosts.CountAsync();
            var pGetFollowPosts = _unitOfWork.Paginate(followPosts, condition.Pagination).ToListAsync();

            // Await all tasks to complete.
            await Task.WhenAll(pGetFollowPosts, pGetFollowPostsCount);

            // Result initialization.
            var result = new SearchResult<IList<FollowPost>>();
            result.Records = pGetFollowPosts.Result;
            result.Total = pGetFollowPostsCount.Result;

            #endregion

            return Ok(result);
        }
        

        #endregion
    }
}