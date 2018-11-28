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
        public AppUnitOfWork(DbContext dbContext, IBaseRepository<User> accounts, IBaseRepository<CategoryGroup> categoryGroups,
            IBaseRepository<Category> categories, IBaseRepository<FollowCategory> followingCategories,
            IBaseRepository<Reply> replies, IBaseRepository<Topic> topics, IBaseRepository<FollowTopic> followingTopics,
            IBaseRepository<ReportTopic> reportTopics, IBaseRepository<NotificationMessage> notificationMessages,
            IBaseRepository<ActivationToken> activationTokens, IBaseRepository<AccessToken> accessTokens,
            IBaseRepository<SignalrConnection> signalrConnections, IBaseRepository<UserRealTimeGroup> userRealTimeGroups,
            IBaseRepository<UserDeviceToken> userDeviceTokens, IBaseRepository<CategorySummary> categorySummaries,
            IBaseRepository<TopicSummary> topicSummaries) : base(dbContext)
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

        public IBaseRepository<User> Accounts { get; }
        public IBaseRepository<CategoryGroup> CategoryGroups { get; }
        public IBaseRepository<Category> Categories { get; }
        public IBaseRepository<FollowCategory> FollowingCategories { get; }
        public IBaseRepository<Reply> Replies { get; }
        public IBaseRepository<Topic> Topics { get; }
        public IBaseRepository<FollowTopic> FollowingTopics { get; }
        public IBaseRepository<ReportTopic> ReportTopics { get; }
        public IBaseRepository<NotificationMessage> NotificationMessages { get; }
        public IBaseRepository<ActivationToken> ActivationTokens { get; }
        public IBaseRepository<AccessToken> AccessTokens { get; }
        public IBaseRepository<SignalrConnection> SignalrConnections { get; }
        public IBaseRepository<UserRealTimeGroup> UserRealTimeGroups { get; }
        public IBaseRepository<UserDeviceToken> UserDeviceTokens { get; }
        public IBaseRepository<CategorySummary> CategorySummaries { get; }
        public IBaseRepository<TopicSummary> TopicSummaries { get; }

        #endregion
    }
}