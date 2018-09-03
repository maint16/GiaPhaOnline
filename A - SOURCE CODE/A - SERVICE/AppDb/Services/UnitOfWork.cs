using System;
using System.Threading.Tasks;
using AppDb.Interfaces;
using AppDb.Interfaces.Repositories;
using AppDb.Models.Entities;
using AppDb.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AppDb.Services
{
    public class UnitOfWork<T> : IUnitOfWork, IDisposable where T : DbContext
    {
        #region Constructors

        /// <summary>
        ///     Initiate unit of work with database context provided by Entity Framework.
        /// </summary>
        /// <param name="dbContext"></param>
        public UnitOfWork(T dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Variables

        /// <summary>
        ///     Whether the instance has been disposed or not.
        /// </summary>
        private bool _disposed;

        /// <summary>
        ///     Provide methods to access confession database.
        /// </summary>
        private readonly DbContext _dbContext;
        #endregion

        #region Properties

        #region Repository instances

        /// <summary>
        ///     Provide access to accounts database.
        /// </summary>
        private IRepository<Account> _accounts;

        /// <summary>
        ///     Provide access to category groups database.
        /// </summary>
        private IRepository<CategoryGroup> _categoryGroups;

        /// <summary>
        ///     Provide access to categories database.
        /// </summary>
        private IRepository<Category> _categories;

        /// <summary>
        /// Provide access to FollowCategory table.
        /// </summary>
        private IRepository<FollowCategory> _followCategories;

        /// <summary>
        ///     Provide access to replys database.
        /// </summary>
        private IRepository<Reply> _replies;

        /// <summary>
        ///     Provide access to topic report database.
        /// </summary>
        private IRepository<ReportTopic> _reportTopics;

        /// <summary>
        ///     Provides access to topic database.
        /// </summary>
        private IRepository<Topic> _topics;

        /// <summary>
        /// Provides access to FollowTopic table.
        /// </summary>
        private IRepository<FollowTopic> _followTopics;

        /// <summary>
        ///     Provide access to token database.
        /// </summary>
        private IRepository<AccessToken> _accessTokens;

        /// <summary>
        ///     Provide access to notification message database.
        /// </summary>
        private IRepository<NotificationMessage> _notificationMessages;

        //        /// <summary>
        //        /// Provide access to categorization database.
        //        /// </summary>
        //        private IRepository<Categorization> _postCategorizations;

        //        /// <summary>
        //        ///     Provide functions to access comment reports database.
        //        /// </summary>
        //        private IRepository<CommentReport> _commentReports;

        //        /// <summary>
        //        /// Provides functions to access comment notification table.
        //        /// </summary>
        //        private IRepository<CommentNotification> _commentNotifications;

        //        /// <summary>
        //        /// Provides access to PostNotification database.
        //        /// </summary>
        //        private IRepository<PostNotification> _postNotifications;

        //        /// <summary>
        //        ///     Provide access to signalr connection database.
        //        /// </summary>
        //        private IRepository<SignalrConnection> _signalrConnections;

        //        /// <summary>
        //        /// Provide access to device database.
        //        /// </summary>
        //        private IRepository<Device> _devices;

        //        /// <summary>
        //        /// List of groups in FCM services.
        //        /// </summary>
        //        private IRepository<FcmGroup> _fcmGroups;

        #endregion

        #region Repository accessors

        /// <summary>
        ///     Provides functions to access account database.
        /// </summary>
        public IRepository<Account> Accounts => _accounts ?? (_accounts = new Repository<Account>(_dbContext));

        /// <summary>
        ///     Provides functions to access category group database.
        /// </summary>
        public IRepository<CategoryGroup> CategoryGroups => _categoryGroups ?? (_categoryGroups = new Repository<CategoryGroup>(_dbContext));

        /// <summary>
        ///     Provides functions to access categories database.
        /// </summary>
        public IRepository<Category> Categories => _categories ?? (_categories = new Repository<Category>(_dbContext));

        /// <summary>
        /// Provides function to access follow category table.
        /// </summary>
        public IRepository<FollowCategory> FollowCategories =>
            _followCategories ?? (_followCategories = new Repository<FollowCategory>(_dbContext));

        /// <summary>
        ///     Provides functions to access reply database.
        /// </summary>
        public IRepository<Reply> Replies => _replies ?? (_replies = new Repository<Reply>(_dbContext));

        /// <summary>
        ///     Provides functions to access to topic reports database.
        /// </summary>
        public IRepository<ReportTopic> ReportTopics
            => _reportTopics ?? (_reportTopics = new Repository<ReportTopic>(_dbContext));

        /// <summary>
        ///     Provides functions to access topic database.
        /// </summary>
        public IRepository<Topic> Topics => _topics ?? (_topics = new Repository<Topic>(_dbContext));

        /// <summary>
        /// Provides function to access FollowTopic table.
        /// </summary>
        public IRepository<FollowTopic> FollowTopics =>
            _followTopics ?? (_followTopics = new Repository<FollowTopic>(_dbContext));

        /// <summary>
        ///     Provides function to access token database.
        /// </summary>
        public IRepository<AccessToken> AccessTokens => _accessTokens ?? (_accessTokens = new Repository<AccessToken>(_dbContext));

        /// <summary>
        ///     Provides function to access notification message database.
        /// </summary>
        public IRepository<NotificationMessage> NotificationMessages => _notificationMessages ?? (_notificationMessages = new Repository<NotificationMessage>(_dbContext));

        //        /// <summary>
        //        /// Provides functions to access categorizations database.
        //        /// </summary>
        //        public IRepository<Categorization> PostCategorizations =>
        //            _postCategorizations ?? (_postCategorizations = new Repository<Categorization>(_dbContext));

        //        /// <summary>
        //        ///     Provides functions to access CommentNotification database.
        //        /// </summary>
        //        public IRepository<CommentNotification> CommentNotifications => _commentNotifications ?? (_commentNotifications = new Repository<CommentNotification>(_dbContext));

        //        /// <summary>
        //        ///     Provides functions to access comment reports database.
        //        /// </summary>
        //        public IRepository<CommentReport> CommentReports => _commentReports ??
        //                                                                      (_commentReports =
        //                                                                          new Repository<CommentReport>(_dbContext));

        //        /// <summary>
        //        /// Provide access to PostNotification table.
        //        /// </summary>
        //        public IRepository<PostNotification> PostNotifications =>
        //            _postNotifications ?? (_postNotifications = new Repository<PostNotification>(_dbContext));

        //        /// <summary>
        //        ///     Provides functions to access realtime connection database.
        //        /// </summary>
        //        public IRepository<SignalrConnection> SignalrConnections => _signalrConnections ??
        //                                                                            (_signalrConnections = new Repository<SignalrConnection>(_dbContext));

        //        /// <summary>
        //        ///     Provides functions to access device database.
        //        /// </summary>
        //        public IRepository<Device> Devices => _devices ?? (_devices = new Repository<Device>(_dbContext));

        //        /// <summary>
        //        ///     List of groups on FCM service.
        //        /// </summary>
        //        public IRepository<FcmGroup> FcmGroups => _fcmGroups ?? (_fcmGroups = new Repository<FcmGroup>(_dbContext));

        #endregion

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
        public async Task<int> CommitAsync()
        {
            return await _dbContext.SaveChangesAsync();
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