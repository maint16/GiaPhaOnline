using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppDb.Models.Entities;
using Shared.Models;
using Shared.ViewModels.FollowTopic;
using Shared.ViewModels.Topic;

namespace AppBusiness.Interfaces.Domains
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
        ///     Delete topic asynchronously.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteTopicAsync(DeleteTopicViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Search topic using id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Topic> GetTopicUsingIdAsync(int id,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Search for topics asynchronously.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SearchResult<IList<Topic>>> SearchTopicsAsync(SearchTopicViewModel condition,
            CancellationToken cancellationToken);

        /// <summary>
        /// Search for topic summaries.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SearchResult<IList<TopicSummary>>> SearchTopicSummaries(SearchTopicSummaryViewModel condition,
            CancellationToken cancellationToken);

        #endregion
    }
}