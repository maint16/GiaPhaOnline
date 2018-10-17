using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppDb.Models.Entities;
using AppShared.Models;
using AppShared.ViewModels.Category;
using SkiaSharp;

namespace AppBusiness.Interfaces.Domains
{
    public interface ICategoryDomain
    {
        #region Methods

        /// <summary>
        ///     Add category asynchronously.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Category> AddCategoryAsync(AddCategoryViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Edit category asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Category> EditCategoryAsync(int id, EditCategoryViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Delete category asynchronously.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteCategoryAsync(DeleteCategoryViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Search categories using specific condition asynchronously.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SearchResult<IList<Category>>> SearchCategoriesAsync(SearchCategoryViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Search category using id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Category> GetCategoryUsingIdAsync(int id,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Search for category summaries asynchronously.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SearchResult<IList<CategorySummary>>> SearchCategorySummariesAsync(SearchCategorySummaryViewModel condition, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Upload category photo asynchronously.
        /// </summary>
        /// <returns></returns>
        Task<Category> UploadCategoryPhotoAsync(int categoryId, SKBitmap photo,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Summarize category information.
        /// </summary>
        /// <returns></returns>
        Task SummarizeCategory();

        #endregion
    }
}