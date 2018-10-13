using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppDb.Models.Entities;
using Shared.Models;
using Shared.ViewModels.Topic;

namespace AppBusiness.Interfaces
{
    public interface ITopicDomain
    {
        #region Methods

        /// <summary>
        ///     Add topic asynchronously into system.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Topic> AddTopicAsync(AddTopicViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Edit topic asynchronously using specific condition & information.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Topic> EditTopicAsync(int id, EditTopicViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Search for topics asynchronously.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SearchResult<IList<Topic>>> SearchTopicsAsync(SearchTopicViewModel condition,
            CancellationToken cancellationToken);

        #endregion
    }
}