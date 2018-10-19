using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServiceShared.Interfaces.Services;

namespace ServiceShared.Services
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        #region Properties

        /// <summary>
        ///     Database set.
        /// </summary>
        private readonly DbSet<T> _dbSet;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initiate repository with database context wrapper.
        /// </summary>
        /// <param name="dbContext"></param>
        public BaseRepository(DbContext dbContext)
        {
            _dbSet = dbContext.Set<T>();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> Search()
        {
            return _dbSet;
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IQueryable<T> Search(Expression<Func<T, bool>> condition)
        {
            return _dbSet.Where(condition);
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> condition,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return _dbSet.FirstOrDefaultAsync(condition, cancellationToken);
        }

        /// <summary>
        ///     Insert a record into data table.
        /// </summary>
        /// <param name="entity"></param>
        public T Insert(T entity)
        {
            return _dbSet.Add(entity).Entity;
        }

        /// <summary>
        ///     Remove a list of entities from database.
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public void Remove(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        /// <summary>
        ///     Remove an entity from database.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        #endregion
    }
}