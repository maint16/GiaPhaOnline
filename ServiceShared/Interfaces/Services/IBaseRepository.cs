using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceShared.Interfaces.Services
{
    public interface IBaseRepository<T>
    {
        #region Methods

        /// <summary>
        ///     Search all data from the specific table.
        /// </summary>
        /// <returns></returns>
        IQueryable<T> Search();

        /// <summary>
        ///     Search data from specific entities set with specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        IQueryable<T> Search(Expression<Func<T, bool>> condition);

        /// <summary>
        ///     Get first result.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> condition,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Insert a record into data table.
        /// </summary>
        /// <param name="entity"></param>
        T Insert(T entity);

        /// <summary>
        ///     Remove a list of entities from database.
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        void Remove(IEnumerable<T> entities);

        /// <summary>
        ///     Remove an entity from database.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        void Remove(T entity);

        #endregion
    }
}