using System;
using System.Data;
using System.Threading.Tasks;
using AppDb.Interfaces;
using AppDb.Interfaces.Repositories;
using AppDb.Models.Entities;
using AppDb.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AppDb.Services
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        #region Constructors

        /// <summary>
        ///     Initiate unit of work with database context provided by Entity Framework.
        /// </summary>
        /// <param name="dbContext"></param>
        public UnitOfWork(DbContext dbContext)
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
        private IRepository<User> _accounts;

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
        ///     Provide access to notification message database.
        /// </summary>
        private IRepository<NotificationMessage> _notificationMessages;

        /// <summary>
        /// Activation token.
        /// </summary>
        private IRepository<ActivationToken> _activationTokens;

        /// <summary>
        /// Access token.
        /// </summary>
        private IRepository<AccessToken> _accessTokens;

        /// <summary>
        ///     Provide access to signalr connection database.
        /// </summary>
        private IRepository<SignalrConnection> _signalrConnections;

        ///// <summary>
        ///// Provides access to signal connection group database.
        ///// </summary>
        //private IRepository<SignalrConnectionGroup> _signalrConnectionGroups;

        private IRepository<UserDeviceToken> _userDeviceTokens;

        #endregion

        #region Repository accessors

        /// <summary>
        ///     Provides functions to access account database.
        /// </summary>
        public IRepository<User> Accounts => _accounts ?? (_accounts = new Repository<User>(_dbContext));

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
        public IRepository<FollowCategory> FollowingCategories =>
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
        public IRepository<FollowTopic> FollowingTopics =>
            _followTopics ?? (_followTopics = new Repository<FollowTopic>(_dbContext));
        
        /// <summary>
        ///     Provides function to access notification message database.
        /// </summary>
        public IRepository<NotificationMessage> NotificationMessages => _notificationMessages ?? (_notificationMessages = new Repository<NotificationMessage>(_dbContext));

        /// <summary>
        /// Provides function to access activation token table.
        /// </summary>
        public IRepository<ActivationToken> ActivationTokens =>
            _activationTokens ?? (_activationTokens = new Repository<ActivationToken>(_dbContext));

        /// <summary>
        /// Provides function to access activation token table.
        /// </summary>
        public IRepository<AccessToken> AccessTokens =>
            _accessTokens ?? (_accessTokens = new Repository<AccessToken>(_dbContext));
        
        /// <summary>
        ///     Provides functions to access realtime connection database.
        /// </summary>
        public IRepository<SignalrConnection> SignalrConnections => _signalrConnections ??
                                                                            (_signalrConnections = new Repository<SignalrConnection>(_dbContext));

        ///// <summary>
        /////     Provides functions to access realtime connection database.
        ///// </summary>
        //public IRepository<SignalrConnectionGroup> SignalrConnectionGroups => _signalrConnectionGroups ??
        //                                                            (_signalrConnectionGroups = new Repository<SignalrConnectionGroup>(_dbContext));

        public IRepository<UserDeviceToken> UserDeviceTokens => _userDeviceTokens ??
                                                                              (_userDeviceTokens = new Repository<UserDeviceToken>(_dbContext));

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
        /// Begin transaction scope.
        /// </summary>
        /// <returns></returns>
        public IDbContextTransaction BeginTransactionScope()
        {
            return _dbContext.Database.BeginTransaction();
        }

        /// <summary>
        /// Begin transaction scope.
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        public IDbContextTransaction BeginTransactionScope(IsolationLevel isolationLevel)
        {
            return _dbContext.Database.BeginTransaction(isolationLevel);
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