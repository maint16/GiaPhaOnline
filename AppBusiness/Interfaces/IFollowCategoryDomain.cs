using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppDb.Models.Entities;
using Shared.Models;
using Shared.ViewModels.FollowCategory;

namespace AppBusiness.Interfaces
{
    public interface IFollowCategoryDomain
    {
        #region Methods

        /// <summary>
        ///     Add follow category asynchronously.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<FollowCategory> AddFollowCategoryAsync(AddFollowCategoryViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Delete follow category asynchronously.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteFollowCategoryAsync(DeleteFollowCategoryViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Search follow categories using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SearchResult<IList<FollowCategory>>> SearchFollowCategoriesAsync(SearchFollowCategoryViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Search follow category using specific condition.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<FollowCategory> SearchFollowCategoryAsync(SearchFollowCategoryViewModel condition,
            CancellationToken cancellationToken);

        #endregion
    }
}