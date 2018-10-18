using AppDb.Interfaces;
using AppDb.Models.Entities;
using Microsoft.EntityFrameworkCore;
using ServiceShared.Interfaces.Services;
using ServiceShared.Services;

namespace AppDb.Services
{
    public class AppUnitOfWork : BaseUnitOfWork, IAppUnitOfWork
    {
        #region Constructors

        /// <summary>
        ///     Initiate unit of work with database context provided by Entity Framework.
        /// </summary>
        public AppUnitOfWork(DbContext dbContext, IRepository<User> accounts, IRepository<CategoryGroup> categoryGroups,
            IRepository<Category> categories, IRepository<FollowCategory> followingCategories,
            IRepository<Reply> replies, IRepository<Topic> topics, IRepository<FollowTopic> followingTopics,
            IRepository<ReportTopic> reportTopics, IRepository<NotificationMessage> notificationMessages,
            IRepository<ActivationToken> activationTokens, IRepository<AccessToken> accessTokens,
            IRepository<SignalrConnection> signalrConnections, IRepository<UserRealTimeGroup> userRealTimeGroups,
            IRepository<UserDeviceToken> userDeviceTokens, IRepository<CategorySummary> categorySummaries,
            IRepository<TopicSummary> topicSummaries) : base(dbContext)
        {
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
    }
}