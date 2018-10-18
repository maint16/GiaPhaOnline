using AppDb.Models.Entities;
using ServiceShared.Interfaces.Services;

namespace AppDb.Interfaces
{
    public interface IAppUnitOfWork : IBaseUnitOfWork
    {
        #region Properties

        IRepository<User> Accounts { get; }
        IRepository<CategoryGroup> CategoryGroups { get; }
        IRepository<Category> Categories { get; }
        IRepository<FollowCategory> FollowingCategories { get; }
        IRepository<Reply> Replies { get; }
        IRepository<Topic> Topics { get; }
        IRepository<FollowTopic> FollowingTopics { get; }
        IRepository<ReportTopic> ReportTopics { get; }
        IRepository<NotificationMessage> NotificationMessages { get; }
        IRepository<ActivationToken> ActivationTokens { get; }
        IRepository<AccessToken> AccessTokens { get; }
        IRepository<SignalrConnection> SignalrConnections { get; }
        IRepository<UserRealTimeGroup> UserRealTimeGroups { get; }
        IRepository<UserDeviceToken> UserDeviceTokens { get; }
        IRepository<CategorySummary> CategorySummaries { get; }
        IRepository<TopicSummary> TopicSummaries { get; }

        #endregion
    }
}