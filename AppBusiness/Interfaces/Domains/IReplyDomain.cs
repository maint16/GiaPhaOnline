using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppDb.Models.Entities;
using AppShared.ViewModels.Reply;
using ClientShared.Models;

namespace AppBusiness.Interfaces.Domains
{
    public interface IReplyDomain
    {
        #region Methods

        /// <summary>
        ///     Add reply asynchronously.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Reply> AddReplyAsync(AddReplyViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Edit reply asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Reply> EditReplyAsync(int id, EditReplyViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Delete reply asynchronously.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteReplyAsync(DeleteReplyViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Search replies asynchronously.
        /// </summary>
        Task<SearchResult<IList<Reply>>> SearchRepliesAsync(SearchReplyViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken));

        #endregion
    }
}