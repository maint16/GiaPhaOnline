using MainDb.Models.Entities;
using ServiceShared.Interfaces.Services;

namespace MainDb.Interfaces
{
    public interface IAppUnitOfWork : IBaseUnitOfWork
    {
        #region Properties

        IBaseRepository<User> Accounts { get; }
        IBaseRepository<CategoryGroup> CategoryGroups { get; }
        IBaseRepository<Category> Categories { get; }
        IBaseRepository<FollowCategory> FollowingCategories { get; }
        IBaseRepository<Reply> Replies { get; }
        IBaseRepository<Topic> Topics { get; }
        IBaseRepository<FollowTopic> FollowingTopics { get; }
        IBaseRepository<ReportTopic> ReportTopics { get; }
        IBaseRepository<NotificationMessage> NotificationMessages { get; }
        IBaseRepository<ActivationToken> ActivationTokens { get; }
        IBaseRepository<AccessToken> AccessTokens { get; }
        IBaseRepository<SignalrConnection> SignalrConnections { get; }
        IBaseRepository<UserRealTimeGroup> UserRealTimeGroups { get; }
        IBaseRepository<UserDeviceToken> UserDeviceTokens { get; }
        IBaseRepository<CategorySummary> CategorySummaries { get; }
        IBaseRepository<TopicSummary> TopicSummaries { get; }

        #endregion
    }
}