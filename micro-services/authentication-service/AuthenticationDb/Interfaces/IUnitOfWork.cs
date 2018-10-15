using System.Data;
using System.Threading;
using System.Threading.Tasks;
using AuthenticationDb.Interfaces.Repositories;
using AuthenticationDb.Models.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace AuthenticationDb.Interfaces
{
    public interface IUnitOfWork
    {
        #region Properties

        /// <summary>
        ///     Provides functions to access account database.
        /// </summary>
        IRepository<User> Accounts { get; }

        #endregion

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
        /// Begin transaction scope.
        /// </summary>
        /// <returns></returns>
        IDbContextTransaction BeginTransactionScope();

        /// <summary>
        /// Begin transaction scope.
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        IDbContextTransaction BeginTransactionScope(IsolationLevel isolationLevel);

        #endregion
    }
}
