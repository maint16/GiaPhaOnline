using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SystemConstant.Enumerations;
using SystemConstant.Enumerations.Order;
using SystemDatabase.Interfaces;
using SystemDatabase.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels;
using Shared.ViewModels.Categories;
using Shared.ViewModels.PostCategorization;

namespace Main.Controllers
{
    /// <summary>
    /// Controller which provides api to handle post categorization.
    /// </summary>
    [Route("api/[controller]")]
    public class PostCategorizationController : Controller
    {
        #region Properties

        /// <summary>
        /// Instance to access to database.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;


        #endregion

        #region Constructors

        /// <summary>
        /// Initialize controller with injectors.
        /// </summary>
        public PostCategorizationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add post categorization into system.
        /// </summary>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<IActionResult> AddPostCategorization([FromBody] AddPostCategorizationViewModel info)
        {
            #region Parameters validation

            if (info == null)
            {
                info = new AddPostCategorizationViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Check categorization duplicate

            // Get all post categorizations.
            var categorizations = _unitOfWork.RepositoryCategorizations.Search();
            categorizations = categorizations.Where(x => x.CategoryId == info.CategoryId && x.PostId == info.PostId);

            // Find post categorization.
            var categorization = await categorizations.FirstOrDefaultAsync();
            if (categorization != null)
                return StatusCode((int)HttpStatusCode.Conflict, new ApiResponse(HttpMessages.PostHasBeenCategorized));

            #endregion

            #region Category check

            // Get all categories in the system.
            var categories = _unitOfWork.RepositoryCategories.Search();
            categories = categories.Where(x => x.Id == info.CategoryId && x.Status == CategoryStatus.Available);

            // Find the first matched result.
            var category = await categories.FirstOrDefaultAsync();

            // Category doesn't exist.
            if (category == null)
                return NotFound(new ApiResponse(HttpMessages.CategoryNotFound));

            #endregion

            #region Post check

            // Get all posts in system.
            var posts = _unitOfWork.RepositoryPosts.Search();
            posts = posts.Where(x => x.Id == info.PostId && x.Status == PostStatus.Active);

            // Get the first post in database.
            var post = await posts.FirstOrDefaultAsync();
            if (post == null)
                return NotFound(new ApiResponse(HttpMessages.PostNotFound));

            #endregion

            #region Categorization initialization

            categorization = new Categorization();
            categorization.CategoryId = category.Id;
            categorization.PostId = post.Id;

            // Add record to database.
            _unitOfWork.RepositoryCategorizations.Insert(categorization);

            // Commit changes.
            await _unitOfWork.CommitAsync();

            #endregion

            return Ok(categorization);
        }

        /// <summary>
        /// Delete post categorization from system.
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [HttpDelete("")]
        public async Task<IActionResult> DeletePostCategorization([FromQuery] int postId, [FromQuery] int categoryId)
        {
            // Get all categorization by using post and category information.
            var categorizations = _unitOfWork.RepositoryCategorizations.Search();
            categorizations = categorizations.Where(x => x.PostId == postId && x.CategoryId == categoryId);

            // Find categorization.
            var categorization = await categorizations.FirstOrDefaultAsync();
            if (categorization == null)
                return NotFound(new ApiResponse(HttpMessages.CategorizationNotFound));

            // Delete the categorization.
            _unitOfWork.RepositoryCategorizations.Remove(categorization);
            return Ok();
        }

        /// <summary>
        /// Search for post notifications.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost("search")]
        public async Task<IActionResult> SearchForPostCategorizations([FromBody] SearchPostCategorizationViewModel info)
        {
            #region Parameters validation

            if (info == null)
            {
                info = new SearchPostCategorizationViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Search for categorizations

            // Search categorizations.
            var categorizations = _unitOfWork.RepositoryCategorizations.Search();

            // Category has been defined.
            if (info.CategoryId != null)
                categorizations = categorizations.Where(x => x.CategoryId == info.CategoryId);

            // Post has been defined.
            if (info.PostId != null)
                categorizations = categorizations.Where(x => x.PostId == info.PostId);

            // Categorization time is defined.
            var categorizationTime = info.CategorizationTime;
            if (categorizationTime != null)
            {
                var from = categorizationTime.From;
                if (from != null)
                    categorizations = _unitOfWork.RepositoryCategorizations.SearchNumericProperty(categorizations,
                        x => x.CategorizationTime, from.Value, NumericComparision.GreaterEqual);

                var to = categorizationTime.To;
                if (to != null)
                    categorizations = _unitOfWork.RepositoryCategorizations.SearchNumericProperty(categorizations,
                        x => x.CategorizationTime, to.Value, NumericComparision.LowerEqual);
            }

            // Sorting defined.
            var sort = info.Sort;
            if (sort != null)
                categorizations =
                    _unitOfWork.RepositoryCategorizations.Sort(categorizations, sort.Direction, sort.Property);
            else
                categorizations = _unitOfWork.RepositoryCategorizations.Sort(categorizations, SortDirection.Decending,
                    PostCategorizationSort.CategorizationTime);

            // Search result initialization.
            var result = new SearchResult<IList<Categorization>>();
            result.Total = await categorizations.CountAsync();

            // Pagination defined.
            var pagination = info.Pagination;
            categorizations = _unitOfWork.RepositoryCategorizations.Paginate(categorizations, pagination);
            result.Records = await categorizations.ToListAsync();

            #endregion

            return Ok(result);
        }

        #endregion
    }
}