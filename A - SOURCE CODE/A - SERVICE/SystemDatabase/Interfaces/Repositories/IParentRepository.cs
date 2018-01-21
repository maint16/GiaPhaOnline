using System.Collections.Generic;
using System.Linq;

namespace SystemDatabase.Interfaces.Repositories
{
    public interface IParentRepository<T> : IDatabaseFunction<T>
    {
        #region Methods
        
            /// <summary>
        ///     Search all data from the specific table.
        /// </summary>
        /// <returns></returns>
        IQueryable<T> Search();

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