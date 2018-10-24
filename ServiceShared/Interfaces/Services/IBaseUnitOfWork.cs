using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace ServiceShared.Interfaces.Services
{
    public interface IBaseUnitOfWork
    {
        #region Methods

        /// <summary>
        ///     Save changes into database.
        /// </summary>
        /// <returns></returns>
        int Commit();

        /// <summary>
        ///     Save changes into database asynchronously.
        /// </summary>
        /// <returns></returns>
        Task<int> CommitAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Begin transaction scope.
        /// </summary>
        /// <returns></returns>
        IDbContextTransaction BeginTransactionScope();

        #endregion
    }
}