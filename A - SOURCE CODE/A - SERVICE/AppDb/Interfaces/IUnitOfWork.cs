using System.Data;
using System.Threading.Tasks;
using AppDb.Interfaces.Repositories;
using AppDb.Models.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace AppDb.Interfaces
{
    public interface IUnitOfWork
    {
        #region Properties

        /// <summary>
        ///     Provides functions to access account database.
        /// </summary>
        IRepository<User> Accounts { get; }

        /// <summary>
        ///     Provides functions to access category group database.
        /// </summary>
        IRepository<CategoryGroup> CategoryGroups { get; }

        /// <summary>
        ///     Provides functions to access category database.
        /// </summary>
        IRepository<Category> Categories { get; }

        /// <summary>
        ///     Provides functions to access follow category table.
        /// </summary>
        IRepository<FollowCategory> FollowingCategories { get; }

        /// <summary>
        ///     Provides functions to access reply database.
        /// </summary>
        IRepository<Reply> Replies { get; }

        /// <summary>
        ///     Provides functions to access topic database.
        /// </summary>
        IRepository<Topic> Topics { get; }

        /// <summary>
        ///     Provides functions to access FollowTopic table.
        /// </summary>
        IRepository<FollowTopic> FollowingTopics { get; }

        /// <summary>
        ///     Provides functions to access topic reports database.
        /// </summary>
        IRepository<ReportTopic> ReportTopics { get; }
        
        /// <summary>
        ///     Provides functions to notification message database.
        /// </summary>
        IRepository<NotificationMessage> NotificationMessages { get; }

        /// <summary>
        ///     Provides functions to notification message database.
        /// </summary>
        IRepository<ActivationToken> ActivationTokens { get; }

        /// <summary>
        /// Access tokens list.
        /// </summary>
        IRepository<AccessToken> AccessTokens { get; }

            /// <summary>
        ///     Provides functions to access signalr connections database.
        /// </summary>
        IRepository<SignalrConnection> SignalrConnections { get; }

        ///// <summary>
        ///// Signal connection groups.
        ///// </summary>
        //IRepository<UserRealTimeGroup> UserRealTimeGroups { get; }

        /// <summary>
        /// Cloud messaging device group.
        /// </summary>
        IRepository<UserDeviceToken> UserDeviceTokens { get; }

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
        Task<int> CommitAsync();

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