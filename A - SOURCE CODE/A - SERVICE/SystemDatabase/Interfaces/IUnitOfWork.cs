using System;
using System.Linq;
using System.Threading.Tasks;
using SystemConstant.Enumerations.Order;
using SystemConstant.Models;
using SystemDatabase.Interfaces.Repositories;
namespace SystemDatabase.Interfaces
{
    public interface IUnitOfWork 
    {
        #region Properties

        /// <summary>
        ///     Provides functions to access account database.
        /// </summary>
        IRepositoryAccount RepositoryAccounts { get; }

        /// <summary>
        ///     Provides functions to access category database.
        /// </summary>
        IRepositoryCategory RepositoryCategories { get; }

        /// <summary>
        /// Provides functions to access follow category table.
        /// </summary>
        IRepositoryFollowCategory RepositoryFollowCategory { get; }

        /// <summary>
        /// Provides function to access categorization database.
        /// </summary>
        IRepositoryCategorization RepositoryCategorizations { get; }

        /// <summary>
        ///     Provides functions to access comment database.
        /// </summary>
        IRepositoryComment RepositoryComments { get; }

        /// <summary>
        ///     Provides functions to access comment reports database.
        /// </summary>
        IRepositoryCommentReport RepositoryCommentReports { get; }

        /// <summary>
        ///     Provides functions to access post reports database.
        /// </summary>
        IRepositoryPost RepositoryPosts { get; }
        
        /// <summary>
        /// Provides functions to access FollowPost table.
        /// </summary>
        IRepositoryFollowPost RepositoryFollowPosts { get; }

        /// <summary>
        ///     Provides functions to access post reports database.
        /// </summary>
        IRepositoryPostReport RepositoryPostReports { get; }

        /// <summary>
        ///     Provides functions to access signalr connections database.
        /// </summary>
        IRepositorySignalrConnection RepositorySignalrConnections { get; }

        /// <summary>
        ///     Provides functions to access token database.
        /// </summary>
        IRepositoryToken RepositoryTokens { get; }

        #endregion

        #region Methods

        
            /// <summary>
        ///     Do pagination on a specific list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        IQueryable<T> Paginate<T>(IQueryable<T> list, Pagination pagination);

        /// <summary>
        ///     Sort a list by using specific property enumeration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="sortDirection"></param>
        /// <param name="sortProperty"></param>
        /// <returns></returns>
        IQueryable<T> Sort<T>(IQueryable<T> list, SortDirection sortDirection, Enum sortProperty);

        /// <summary>
        ///     Search property base on searching mode.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="records"></param>
        /// <param name="property"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        IQueryable<T> SearchPropertyText<T>(IQueryable<T> records, Func<T, string> property, TextSearch search);

        /// <summary>
        ///     Save changes into database.
        /// </summary>
        /// <returns></returns>
        int Commit();

        /// <summary>
        ///     Save changes into database asynchronously.
        /// </summary>
        /// <returns></returns>
        Task<int> CommitAsync();

        #endregion
    }
}