using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppDb.Models.Entities;
using Main.ViewModels.Category;
using Shared.Models;

namespace Main.Interfaces.Services.Businesses
{
    public interface ICategoryService
    {
        #region Methods

        /// <summary>
        /// Add category asynchronously.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Category> AddCategoryAsync(AddCategoryViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Edit category asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Category> EditCategoryAsync(int id, EditCategoryViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Search categories using specific condition asynchronously.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SearchResult<IList<Category>>> SearchCategoriesAsync(SearchCategoryViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Search category using id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Category> GetCategoryUsingIdAsync(int id, CancellationToken cancellationToken = default(CancellationToken));

        #endregion
    }
}