using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppDb.Models.Entities;
using Shared.Models;
using Shared.ViewModels.FollowTopic;

namespace AppBusiness.Interfaces
{
    public interface IFollowTopicDomain
    {
        #region Methods

        /// <summary>
        ///     Start following topic.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<FollowTopic> AddFollowTopicAsync(AddFollowTopicViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Stop following topic.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteFollowTopicAsync(DeleteFollowTopicViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Get follow topics asynchronously.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SearchResult<IList<FollowTopic>>> SearchFollowTopicsAsync(SearchFollowTopicViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken));

        #endregion
    }
}