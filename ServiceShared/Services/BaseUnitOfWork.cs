using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ServiceShared.Interfaces.Services;

namespace ServiceShared.Services
{
    public class BaseUnitOfWork : IBaseUnitOfWork, IDisposable
    {
        #region Constructors

        /// <summary>
        ///     Initiate unit of work with database context provided by Entity Framework.
        /// </summary>
        public BaseUnitOfWork(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Whether the instance has been disposed or not.
        /// </summary>
        private bool _disposed;

        /// <summary>
        ///     Provide methods to access confession database.
        /// </summary>
        private readonly DbContext _dbContext;

        #endregion

        #region Methods

        /// <summary>
        ///     Save changes into database.
        /// </summary>
        /// <returns></returns>
        public int Commit()
        {
            return _dbContext.SaveChanges();
        }

        /// <summary>
        ///     Save changes into database asynchronously.
        /// </summary>
        /// <returns></returns>
        public async Task<int> CommitAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        ///     Begin transaction scope.
        /// </summary>
        /// <returns></returns>
        public IDbContextTransaction BeginTransactionScope()
        {
            return _dbContext.Database.BeginTransaction();
        }

        /// <summary>
        ///     Dispose the instance and free it from memory.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            // Object has been disposed.
            if (_disposed)
                return;

            // Object is being disposed.
            if (disposing)
                _dbContext.Dispose();

            _disposed = true;
        }

        /// <summary>
        ///     Dispose the instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}