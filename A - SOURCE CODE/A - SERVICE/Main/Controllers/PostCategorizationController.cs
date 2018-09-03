using AppDb.Interfaces;
using AppDb.Interfaces.Repositories;
using Main.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Interfaces.Services;

namespace Main.Controllers
{
    /// <summary>
    ///     Controller which provides api to handle post categorization.
    /// </summary>
    [Route("api/post-categorization")]
    public class PostCategorizationController : Controller
    {
        #region Constructors

        /// <summary>
        ///     Initialize controller with injectors.
        /// </summary>
        public PostCategorizationController(IUnitOfWork unitOfWork, IRelationalDbService databaseFunction)
        {
            _unitOfWork = unitOfWork;
            _databaseFunction = databaseFunction;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Instance to access to database.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        ///     Instance to access database function.
        /// </summary>
        private readonly IRelationalDbService _databaseFunction;

        #endregion

        //#region Methods

        ///// <summary>
        ///// Add post categorization into system.
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost("")]
        //public async Task<IActionResult> AddPostCategorization([FromBody] AddPostCategorizationViewModel info)
        //{
        //    #region Parameters validation

        //    if (info == null)
        //    {
        //        info = new AddPostCategorizationViewModel();
        //        TryValidateModel(info);
        //    }

        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    #endregion

        //    #region Check categorization duplicate

        //    // Get all post categorizations.
        //    var categorizations = _unitOfWork.PostCategorizations.Search();
        //    categorizations = categorizations.Where(x => x.CategoryId == info.CategoryId && x.PostId == info.PostId);

        //    // Find post categorization.
        //    var categorization = await categorizations.FirstOrDefaultAsync();
        //    if (categorization != null)
        //        return StatusCode((int)HttpStatusCode.Conflict, new ApiResponse(HttpMessages.PostHasBeenCategorized));

        //    #endregion

        //    #region Category check

        //    // Get all categories in the system.
        //    var categories = _unitOfWork.Categories.Search();
        //    categories = categories.Where(x => x.Id == info.CategoryId && x.Status == ItemStatus.Available);

        //    // Find the first matched result.
        //    var category = await categories.FirstOrDefaultAsync();

        //    // Category doesn't exist.
        //    if (category == null)
        //        return NotFound(new ApiResponse(HttpMessages.CategoryNotFound));

        //    #endregion

        //    #region Post check

        //    // Get all posts in system.
        //    var posts = _unitOfWork.Posts.Search();
        //    posts = posts.Where(x => x.Id == info.PostId && x.Status == PostStatus.Available);

        //    // Get the first post in database.
        //    var post = await posts.FirstOrDefaultAsync();
        //    if (post == null)
        //        return NotFound(new ApiResponse(HttpMessages.PostNotFound));

        //    #endregion

        //    #region Categorization initialization

        //    categorization = new Categorization();
        //    categorization.CategoryId = category.Id;
        //    categorization.PostId = post.Id;

        //    // Add record to database.
        //    _unitOfWork.PostCategorizations.Insert(categorization);

        //    // Commit changes.
        //    await _unitOfWork.CommitAsync();

        //    #endregion

        //    return Ok(categorization);
        //}

        ///// <summary>
        ///// Delete post categorization from system.
        ///// </summary>
        ///// <param name="postId"></param>
        ///// <param name="categoryId"></param>
        ///// <returns></returns>
        //[HttpDelete("")]
        //public async Task<IActionResult> DeletePostCategorization([FromRoute] int postId, [FromRoute] int categoryId)
        //{
        //    // Get all categorization by using post and category information.
        //    var categorizations = _unitOfWork.PostCategorizations.Search();
        //    categorizations = categorizations.Where(x => x.PostId == postId && x.CategoryId == categoryId);

        //    // Find categorization.
        //    var categorization = await categorizations.FirstOrDefaultAsync();
        //    if (categorization == null)
        //        return NotFound(new ApiResponse(HttpMessages.CategorizationNotFound));

        //    // Delete the categorization.
        //    _unitOfWork.PostCategorizations.Remove(categorization);
        //    return Ok();
        //}

        ///// <summary>
        ///// Search for post notifications.
        ///// </summary>
        ///// <param name="info"></param>
        ///// <returns></returns>
        //[HttpPost("search")]
        //[ByPassAuthorization]
        //public async Task<IActionResult> SearchForPostCategorizations([FromBody] SearchPostCategorizationViewModel info)
        //{
        //    #region Parameters validation

        //    if (info == null)
        //    {
        //        info = new SearchPostCategorizationViewModel();
        //        TryValidateModel(info);
        //    }

        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    #endregion

        //    #region Search for categorizations

        //    // Search categorizations.
        //    var categorizations = _unitOfWork.PostCategorizations.Search();

        //    // Category has been defined.
        //    if (info.CategoryIds != null && info.CategoryIds.Count > 0)
        //    {
        //        var categoryIds = info.CategoryIds.Where(x => x > 0).ToList();
        //        if (categoryIds.Count > 0)
        //            categorizations = categorizations.Where(x => info.CategoryIds.Contains(x.CategoryId));
        //    }

        //    // Post has been defined.
        //    if (info.PostIds != null && info.PostIds.Count > 0)
        //    {
        //        var pategoryIds = info.PostIds.Where(x => x > 0).ToList();
        //        if (pategoryIds.Count > 0)
        //            categorizations = categorizations.Where(x => info.PostIds.Contains(x.PostId));
        //    }

        //    // Categorization time is defined.
        //    var categorizationTime = info.CategorizationTime;
        //    if (categorizationTime != null)
        //    {
        //        var from = categorizationTime.From;
        //        if (from != null)
        //            categorizations = _databaseFunction.SearchNumericProperty(categorizations,
        //                x => x.CategorizationTime, from.Value, NumericComparision.GreaterEqual);

        //        var to = categorizationTime.To;
        //        if (to != null)
        //            categorizations = _databaseFunction.SearchNumericProperty(categorizations,
        //                x => x.CategorizationTime, to.Value, NumericComparision.LowerEqual);
        //    }

        //    // Sorting defined.
        //    var sort = info.Sort;
        //    if (sort != null)
        //        categorizations =
        //            _databaseFunction.Sort(categorizations, sort.Direction, sort.Property);
        //    else
        //        categorizations = _databaseFunction.Sort(categorizations, SortDirection.Decending,
        //            PostCategorizationSort.CategorizationTime);

        //    // Search result initialization.
        //    var result = new SearchResult<IList<Categorization>>();
        //    result.Total = await categorizations.CountAsync();

        //    // Pagination defined.
        //    var pagination = info.Pagination;
        //    categorizations = _databaseFunction.Paginate(categorizations, pagination);
        //    result.Records = await categorizations.ToListAsync();

        //    #endregion

        //    return Ok(result);
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="condition"></param>
        ///// <returns></returns>
        //[HttpPost("count-posts")]
        //[ByPassAuthorization]
        //public async Task<IActionResult> GetCategoryPostsCounter([FromBody] CountPostCategorizationViewModel condition)
        //{
        //    #region Parameters validation

        //    if (condition == null)
        //    {
        //        condition = new CountPostCategorizationViewModel();
        //        TryValidateModel(condition);
        //    }

        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    #endregion

        //    #region Search for information

        //    // Get all follow posts
        //    var postCategorizations = _unitOfWork.PostCategorizations.Search();

        //    if (condition.CategoryIds != null && condition.CategoryIds.Count > 0)
        //    {
        //        var categoryIds = condition.CategoryIds.Where(x => x > 0).ToList();
        //        if (categoryIds.Count > 0)
        //            postCategorizations = postCategorizations.Where(x => condition.CategoryIds.Contains(x.CategoryId));
        //    }

        //    var categoryGroups = postCategorizations.GroupBy(x => x.CategoryId).Select(x => new CategoryPostCounterModel
        //    {
        //        CategoryId = x.Key,
        //        TotalPosts = x.Count()
        //    });


        //    // Sort by properties.
        //    if (condition.Sort != null)
        //        categoryGroups =
        //            _databaseFunction.Sort(categoryGroups, condition.Sort.Direction,
        //                condition.Sort.Property);
        //    else
        //        categoryGroups = _databaseFunction.Sort(categoryGroups, SortDirection.Ascending,
        //            PostCategorizationSort.CategoryId);

        //    // Result initialization.
        //    var result = new SearchResult<IList<CategoryPostCounterModel>>();
        //    result.Total = categoryGroups.Count(model => model.CategoryId > 0);
        //    result.Records = await _databaseFunction.Paginate(categoryGroups, condition.Pagination).ToListAsync();

        //    #endregion
        //    return Ok(result);
        //}

        /////// <summary>
        ///////     Load categorizations by using specific conditions.
        /////// </summary>
        /////// <param name="categorizations"></param>
        /////// <param name="conditions"></param>
        /////// <returns></returns>
        ////public IQueryable<Categorization> GetCategoryPostsCounter(IQueryable<Categorization> categorizations,
        ////    CountPostCategorizationViewModel conditions)
        ////{
        ////    if (conditions == null)
        ////        return categorizations;

        ////    // PostId has been defined.
        ////    if (conditions.CategoryIds != null && conditions.CategoryIds.Count > 0)
        ////    {
        ////        var categoryIds = conditions.CategoryIds.Where(x => x > 0).ToList();
        ////        if (categoryIds.Count > 0)
        ////            categorizations = categorizations.Where(x => conditions.CategoryIds.Contains(x.CategoryId));
        ////    }

        ////    var a = categorizations.GroupBy(x => x.CategoryId).Select(x => new
        ////    {
        ////        CategoryId = x.Key,
        ////        Total = x.Count()
        ////    });

        ////    return categorizations;
        ////}

        //#endregion
    }
}