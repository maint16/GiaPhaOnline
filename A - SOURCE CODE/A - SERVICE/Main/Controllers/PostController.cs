using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemConstant.Enumerations;
using SystemConstant.Enumerations.Order;
using SystemDatabase.Interfaces;
using SystemDatabase.Models.Entities;
using AutoMapper;
using Main.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Services;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels;
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

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize controller with injectors.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="mapper"></param>
        /// <param name="identityService"></param>
        /// <param name="timeService"></param>
        public PostController(IUnitOfWork unitOfWork, IMapper mapper, IIdentityService identityService, ITimeService timeService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _identityService = identityService;
            _timeService = timeService;
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
            _unitOfWork.RepositoryPosts.Insert(post);

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
        public async Task<IActionResult> EditPost([FromQuery] int id, [FromBody] EditPostViewModel info)
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

            var posts = _unitOfWork.RepositoryPosts.Search();
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
        public async Task<IActionResult> DeletePost([FromQuery] int id)
        {
            // Find post by using post.
            var posts = _unitOfWork.RepositoryPosts.Search();

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
            var posts = _unitOfWork.RepositoryPosts.Search();

            // Id is defined.
            if (condition.Id != null)
                posts = posts.Where(x => x.Id == condition.Id);
            
            // Owner is defined.
            if (condition.OwnerId != null)
                posts = posts.Where(x => x.OwnerId == condition.OwnerId);
            
            // Search conditions which are based on roles.
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
            
            // Created time has been defined.
            var createdTime = condition.CreatedTime;
            if (createdTime != null)
            {
                var from = createdTime.From;
                var to = createdTime.To;

                if (from != null)
                    posts = _unitOfWork.RepositoryPosts.SearchNumericProperty(posts, x => x.CreatedTime, from.Value,
                        NumericComparision.GreaterEqual);
                
                if (to != null)
                    posts = _unitOfWork.RepositoryPosts.SearchNumericProperty(posts, x => x.CreatedTime, to.Value,
                        NumericComparision.LowerEqual);
            }

            // Created time has been defined.
            var lastModifiedTime = condition.LastModifiedTime;
            if (lastModifiedTime != null)
            {
                var from = lastModifiedTime.From;
                var to = lastModifiedTime.To;

                if (from != null)
                    posts = _unitOfWork.RepositoryPosts.SearchNumericProperty(posts, x => x.LastModifiedTime, from.Value,
                        NumericComparision.GreaterEqual);

                if (to != null)
                    posts = _unitOfWork.RepositoryPosts.SearchNumericProperty(posts, x => x.LastModifiedTime, to.Value,
                        NumericComparision.LowerEqual);
            }

            // Sort property & direction.
            var sort = condition.Sort;
            if (sort != null)
                posts = _unitOfWork.Sort(posts, sort.Direction, sort.Property);
            else
                posts = _unitOfWork.Sort(posts, SortDirection.Decending, SortDirection.Decending);

            // Count posts.
            var pGetPostCount = posts.CountAsync();
            var pGetPosts = _unitOfWork.Paginate(posts, condition.Pagination).ToListAsync();

            await Task.WhenAll(pGetPostCount, pGetPosts);
            var result = new SearchResult<IList<Post>>();
            result.Records = pGetPosts.Result;
            result.Total = pGetPostCount.Result;

            #endregion

            return Ok(result);
        }
        
        #endregion
    }
}