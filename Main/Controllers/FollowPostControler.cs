﻿using AppDb.Interfaces;
using AppDb.Interfaces.Repositories;
using AutoMapper;
using Main.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Interfaces.Services;

namespace Main.Controllers
{
    [Route("api/follow-post")]
    public class FollowPostController : ApiBaseController
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
        public FollowPostController(IUnitOfWork unitOfWork, IMapper mapper, ITimeService timeService,
            IRelationalDbService relationalDbService, IIdentityService identityService) : base(unitOfWork, mapper, timeService,
            relationalDbService, identityService)
        {
            _unitOfWork = unitOfWork;
            _databaseFunction = relationalDbService;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Instance for accessing database.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        ///     Provide access to generic database functions.
        /// </summary>
        private readonly IRelationalDbService _databaseFunction;

        #endregion

        //#region Methods

        ///// <summary>
        /////     Add a category into database.
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost("")]
        //public async Task<IActionResult> StartFollowingPost([FromBody] FollowPostViewModel info)
        //{
        //    #region Find post

        //    // Get posts by using id.
        //    var posts = UnitOfWork.Topics.Search();
        //    posts = posts.Where(x => x.Id == info.PostId && x.Status == PostStatus.Available);
        //    var post = await posts.FirstOrDefaultAsync();

        //    // Post is not found.
        //    if (post == null)
        //        return NotFound(new ApiResponse(HttpMessages.PostNotFound));

        //    #endregion

        //    #region Check following duplicate

        //    // Get identity from request.
        //    var identity = IdentityService.GetProfile(HttpContext);

        //    // Get follow posts.
        //    var followPosts = UnitOfWork.FollowingTopics.Search();
        //    followPosts = followPosts.Where(x => x.FollowerId == identity.Id && x.PostId == post.Id);
        //    var followPost = await followPosts.FirstOrDefaultAsync();

        //    if (followPost != null)
        //    {
        //        followPost.Status = ItemStatus.Available;
        //    }
        //    else
        //    {
        //        followPost = new FollowPost();
        //        followPost.FollowerId = identity.Id;
        //        followPost.PostId = info.PostId;
        //        followPost.Status = ItemStatus.Available;
        //        followPost.CreatedTime = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);

        //        // Insert record into system.
        //        UnitOfWork.FollowingTopics.Insert(followPost);
        //    }

        //    // Commit changes to system.
        //    await UnitOfWork.CommitAsync();

        //    #endregion

        //    return Ok(followPost);
        //}

        ///// <summary>
        /////     Stop following a specific post.
        ///// </summary>
        ///// <param name="postId"></param>
        ///// <returns></returns>
        //[HttpDelete]
        //public async Task<IActionResult> StopFollowingPost([FromQuery] int postId)
        //{
        //    #region Check following duplicate

        //    // Get identity from request.
        //    var identity = IdentityService.GetProfile(HttpContext);

        //    // Get follow posts.
        //    var followPosts = UnitOfWork.FollowingTopics.Search();
        //    followPosts = followPosts.Where(x => x.FollowerId == identity.Id && x.PostId == postId);
        //    var followPost = await followPosts.FirstOrDefaultAsync();

        //    if (followPost == null)
        //        return NotFound(new ApiResponse(HttpMessages.PostHasntBeenFollowedYet));

        //    // Update follow post.
        //    followPost.Status = ItemStatus.NotAvailable;

        //    // Commit changes to system.
        //    await UnitOfWork.CommitAsync();

        //    #endregion

        //    return Ok(followPost);
        //}

        ///// <summary>
        /////     Search for a list of categories.
        ///// </summary>
        ///// <param name="condition"></param>
        ///// <returns></returns>
        //[HttpPost("search")]
        //[ByPassAuthorization]
        //public async Task<IActionResult> SearchFollowingPosts([FromBody] SearchFollowPostViewModel condition)
        //{
        //    #region Parameters validation

        //    if (condition == null)
        //    {
        //        condition = new SearchFollowPostViewModel();
        //        TryValidateModel(condition);
        //    }

        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    #endregion

        //    #region Search for information

        //    // Find request identity.
        //    var identity = IdentityService.GetProfile(HttpContext);

        //    // Get all categories.
        //    var followPosts = UnitOfWork.FollowingTopics.Search();

        //    // Post id is defined.
        //    if (condition.PostId != null)
        //        followPosts = followPosts.Where(x => x.PostId == condition.PostId.Value);

        //    // Statuses are defined.
        //    if (condition.Statuses != null && condition.Statuses.Count > 0)
        //    {
        //        condition.Statuses = condition.Statuses.Where(x => Enum.IsDefined(typeof(ItemStatus), x))
        //            .ToHashSet();

        //        if (condition.Statuses.Count > 0)
        //            followPosts = followPosts.Where(x => condition.Statuses.Contains(x.Status));
        //    }

        //    // Only see the posts that user is following.
        //    followPosts = followPosts.Where(x => x.FollowerId == identity.Id);

        //    // Created time is defined.
        //    var createdTime = condition.CreatedTime;
        //    if (createdTime != null)
        //    {
        //        var from = createdTime.From;
        //        var to = createdTime.To;

        //        if (from != null)
        //            followPosts = RelationalDbService.SearchNumericProperty(followPosts,
        //                x => x.CreatedTime, from.Value, NumericComparision.GreaterEqual);

        //        if (to != null)
        //            followPosts = RelationalDbService.SearchNumericProperty(followPosts,
        //                x => x.CreatedTime, to.Value, NumericComparision.LowerEqual);
        //    }

        //    // Sort by properties.
        //    if (condition.Sort != null)
        //        followPosts =
        //            RelationalDbService.Sort(followPosts, condition.Sort.Direction,
        //                condition.Sort.Property);
        //    else
        //        followPosts = RelationalDbService.Sort(followPosts, SortDirection.Decending,
        //            FollowPostSort.CreatedTime);

        //    #endregion

        //    #region Result gathering

        //    // Result initialization.
        //    var result = new SearchResult<IList<FollowPost>>();
        //    result.Total = await followPosts.CountAsync();
        //    result.Records = await RelationalDbService.Paginate(followPosts, condition.Pagination).ToListAsync();

        //    #endregion

        //    return Ok(result);
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="condition"></param>
        ///// <returns></returns>
        //[HttpPost("load")]
        //public async Task<IActionResult> LoadFollowingPosts([FromBody]LoadFollowPostViewModel condition)
        //{
        //    #region Parameters validation

        //    if (condition == null)
        //    {
        //        condition = new LoadFollowPostViewModel();
        //        TryValidateModel(condition);
        //    }

        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    #endregion

        //    #region Search for information

        //    // Get all follow posts
        //    var followPosts = _unitOfWork.FollowingTopics.Search();
        //    followPosts = LoadFollowingPosts(followPosts, condition);

        //    // Sort by properties.
        //    if (condition.Sort != null)
        //        followPosts =
        //            _databaseFunction.Sort(followPosts, condition.Sort.Direction,
        //                condition.Sort.Property);
        //    else
        //        followPosts = _databaseFunction.Sort(followPosts, SortDirection.Decending,
        //            FollowPostSort.CreatedTime);

        //    // Result initialization.
        //    var result = new SearchResult<IList<FollowPost>>();
        //    result.Total = await followPosts.CountAsync();
        //    result.Records = await _databaseFunction.Paginate(followPosts, condition.Pagination).ToListAsync();

        //    #endregion

        //    return Ok(result);
        //}

        ///// <summary>
        /////     Load follow post by using specific conditions.
        ///// </summary>
        ///// <param name="followPosts"></param>
        ///// <param name="conditions"></param>
        ///// <returns></returns>
        //public IQueryable<FollowPost> LoadFollowingPosts(IQueryable<FollowPost> followPosts,
        //    LoadFollowPostViewModel conditions)
        //{
        //    if (conditions == null)
        //        return followPosts;

        //    // PostId has been defined.
        //    if (conditions.PostIds != null && conditions.PostIds.Count > 0)
        //    {
        //        conditions.PostIds = conditions.PostIds.Where(x => x > 0).ToList();
        //        if (conditions.PostIds.Count > 0)
        //            followPosts = followPosts.Where(x => conditions.PostIds.Contains(x.PostId));
        //    }

        //    return followPosts;
        //}

        //#endregion
    }
}