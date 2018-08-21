using System.Threading.Tasks;
using AppDb.Interfaces.Repositories;
using AppDb.Models.Entities;

namespace AppDb.Interfaces
{
    public interface IUnitOfWork
    {
        #region Properties

        /// <summary>
        ///     Provides functions to access account database.
        /// </summary>
        IRepository<Account> Accounts { get; }

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
        IRepository<FollowCategory> FollowCategories { get; }

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
        IRepository<FollowTopic> FollowTopics { get; }

        /// <summary>
        ///     Provides functions to access topic reports database.
        /// </summary>
        IRepository<ReportTopic> ReportTopics { get; }

        /// <summary>
        ///     Provides functions to access token database.
        /// </summary>
        IRepository<AccessToken> AccessTokens { get; }

        /// <summary>
        ///     Provides functions to notification message database.
        /// </summary>
        IRepository<NotificationMessage> NotificationMessages { get; }

        //        /// <summary>
        //        ///     Provides function to access categorization database.
        //        /// </summary>
        //        IRepository<Categorization> PostCategorizations { get; }



        //        /// <summary>
        //        ///     Provides functions to access comment reports database.
        //        /// </summary>
        //        IRepository<CommentReport> CommentReports { get; }

        //        /// <summary>
        //        /// Provides functions to access CommentNotification table.
        //        /// </summary>
        //        IRepository<CommentNotification> CommentNotifications { get; }

        //        /// <summary>
        //        /// Provide functions to access PostNotification table.
        //        /// </summary>
        //        IRepository<PostNotification> PostNotifications { get; }

        //        /// <summary>
        //        ///     Provides functions to access signalr connections database.
        //        /// </summary>
        //        IRepository<SignalrConnection> SignalrConnections { get; }



        //        /// <summary>
        //        /// Provides function to access Device datablase.
        //        /// </summary>
        //        IRepository<Device> Devices { get; }

        //        /// <summary>
        //        /// List of groups in FCM service.
        //        /// </summary>
        //        IRepository<FcmGroup> FcmGroups { get; }

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

        #endregion
    }
}