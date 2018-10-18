using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using AppDb.Interfaces;
using AppDb.Interfaces.Repositories;
using AppDb.Models.Entities;
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
        public UnitOfWork(DbContext dbContext, IRepository<User> accounts, IRepository<CategoryGroup> categoryGroups,
            IRepository<Category> categories, IRepository<FollowCategory> followingCategories,
            IRepository<Reply> replies, IRepository<Topic> topics, IRepository<FollowTopic> followingTopics,
            IRepository<ReportTopic> reportTopics, IRepository<NotificationMessage> notificationMessages,
            IRepository<ActivationToken> activationTokens, IRepository<AccessToken> accessTokens,
            IRepository<SignalrConnection> signalrConnections, IRepository<UserRealTimeGroup> userRealTimeGroups,
            IRepository<UserDeviceToken> userDeviceTokens, IRepository<CategorySummary> categorySummaries,
            IRepository<TopicSummary> topicSummaries)
        {
            _dbContext = dbContext;
            Accounts = accounts;
            CategoryGroups = categoryGroups;
            Categories = categories;
            FollowingCategories = followingCategories;
            Replies = replies;
            Topics = topics;
            FollowingTopics = followingTopics;
            ReportTopics = reportTopics;
            NotificationMessages = notificationMessages;
            ActivationTokens = activationTokens;
            AccessTokens = accessTokens;
            SignalrConnections = signalrConnections;
            UserRealTimeGroups = userRealTimeGroups;
            UserDeviceTokens = userDeviceTokens;
            CategorySummaries = categorySummaries;
            TopicSummaries = topicSummaries;
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

        public IRepository<User> Accounts { get; }
        public IRepository<CategoryGroup> CategoryGroups { get; }
        public IRepository<Category> Categories { get; }
        public IRepository<FollowCategory> FollowingCategories { get; }
        public IRepository<Reply> Replies { get; }
        public IRepository<Topic> Topics { get; }
        public IRepository<FollowTopic> FollowingTopics { get; }
        public IRepository<ReportTopic> ReportTopics { get; }
        public IRepository<NotificationMessage> NotificationMessages { get; }
        public IRepository<ActivationToken> ActivationTokens { get; }
        public IRepository<AccessToken> AccessTokens { get; }
        public IRepository<SignalrConnection> SignalrConnections { get; }
        public IRepository<UserRealTimeGroup> UserRealTimeGroups { get; }
        public IRepository<UserDeviceToken> UserDeviceTokens { get; }
        public IRepository<CategorySummary> CategorySummaries { get; }
        public IRepository<TopicSummary> TopicSummaries { get; }

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
        ///     Begin transaction scope.
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