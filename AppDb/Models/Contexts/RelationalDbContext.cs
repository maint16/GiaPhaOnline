using System.Linq;
using System.Threading.Tasks;
using AppDb.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AppDb.Models.Contexts
{
    public class RelationalDbContext : DbContext
    {
        #region Constructors

        /// <summary>
        ///     Initiate database context with connection string.
        /// </summary>
        public RelationalDbContext(DbContextOptions dbContextOptions)
            : base(dbContextOptions)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///     List of accounts in database.
        /// </summary>
        public virtual DbSet<User> Users { get; set; }

        /// <summary>
        ///     List of category groups in database.
        /// </summary>
        public virtual DbSet<CategoryGroup> CategoryGroups { get; set; }

        /// <summary>
        ///     List of categories in database.
        /// </summary>
        public virtual DbSet<Category> Categories { get; set; }

        /// <summary>
        ///     List of relationships between followers and categories. (many - many)
        /// </summary>
        public virtual DbSet<FollowCategory> FollowCategories { get; set; }

        /// <summary>
        ///     List of relationships between followers and posts (many - many).
        /// </summary>
        public virtual DbSet<FollowTopic> FollowTopics { get; set; }

        /// <summary>
        ///     List of posts.
        /// </summary>
        public virtual DbSet<Topic> Topics { get; set; }

        /// <summary>
        ///     List of post reports.
        /// </summary>
        public virtual DbSet<ReportTopic> ReportTopics { get; set; }

        /// <summary>
        ///     List of notification messages in database.
        /// </summary>
        public virtual DbSet<NotificationMessage> NotificationMessages { get; set; }

        /// <summary>
        ///     Topic replies.
        /// </summary>
        public virtual DbSet<Reply> Replies { get; set; }

        /// <summary>
        ///     Signalr connection.
        /// </summary>
        public virtual DbSet<SignalrConnection> SignalrConnections { get; set; }

        /// <summary>
        ///     User real time groups.
        /// </summary>
        public virtual DbSet<UserRealTimeGroup> UserRealTimeGroups { get; set; }

        /// <summary>
        ///     List of device groups.
        /// </summary>
        public virtual DbSet<UserDeviceToken> UserDeviceTokens { get; set; }

        /// <summary>
        /// Category summaries.
        /// </summary>
        public virtual DbSet<CategorySummary> CategorySummaries { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Save changes into database.
        /// </summary>
        /// <returns></returns>
        public int Commit()
        {
            return SaveChanges();
        }

        /// <summary>
        ///     Save changes into database asynchronously.
        /// </summary>
        /// <returns></returns>
        public async Task<int> CommitAsync()
        {
            return await SaveChangesAsync();
        }

        /// <summary>
        ///     Callback which is fired when model is being created.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User table.
            AddUserTable(modelBuilder);

            // Category group table.
            AddCategoryGroupTable(modelBuilder);

            // Category table.
            AddCategoryTable(modelBuilder);

            // Topic table.
            AddTopicTable(modelBuilder);

            // Reply table.
            AddReplyTable(modelBuilder);

            // Activation token.
            AddActivationTokenTable(modelBuilder);

            // Add access token.
            AddAccessTokenTable(modelBuilder);

            // Follow category table.
            AddFollowCategoryTable(modelBuilder);

            // Follow topic table.
            AddFollowTopicTable(modelBuilder);

            // Report topic table.
            AddReportTopicTable(modelBuilder);

            // Add signalr connection table.
            AddSignalrConnectionTable(modelBuilder);

            // Add signalr connection group table.
            AddUserDeviceGroupTable(modelBuilder);

            // Add notification message table.
            AddNotificationMessageTable(modelBuilder);

            // Add cloud messaging device group table.
            AddCloudMessagingDeviceGroupTable(modelBuilder);

            // Add category summary table.
            AddCategorySummaryTable(modelBuilder);

            AddTopicSummaryTable(modelBuilder);

            // Use model builder to specify composite primary keys.
            // Composite primary keys configuration

            // This is for remove pluralization naming convention in database defined by Entity Framework.
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
                entity.Relational().TableName = entity.DisplayName();

            // Disable cascade delete.
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }

        #region Tables initialization

        /// <summary>
        ///     Initialize account table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void AddUserTable(ModelBuilder modelBuilder)
        {
            var user = modelBuilder.Entity<User>();

            // Set primary key.
            user.HasKey(x => x.Id);
            user.Property(x => x.Id).UseSqlServerIdentityColumn();

            user.Property(x => x.Email).IsRequired();
            user.Property(x => x.Nickname).IsRequired();
        }

        /// <summary>
        ///     Initialize category table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void AddCategoryGroupTable(ModelBuilder modelBuilder)
        {
            var categoryGroup = modelBuilder.Entity<CategoryGroup>();

            // Set primary key.
            categoryGroup.HasKey(x => x.Id);
            categoryGroup.Property(x => x.Id).UseSqlServerIdentityColumn();

            // Relationship between category group & account.
            categoryGroup.HasOne(x => x.Creator).WithMany(x => x.CategoryGroups).HasForeignKey(x => x.CreatorId);
        }

        /// <summary>
        ///     Initialize category table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void AddCategoryTable(ModelBuilder modelBuilder)
        {
            var category = modelBuilder.Entity<Category>();

            // Set primary key.
            category.HasKey(x => x.Id);
            category.Property(x => x.Id).UseSqlServerIdentityColumn();

            // Relationship between category & account.
            category.HasOne(x => x.Creator).WithMany(x => x.Categories).HasForeignKey(x => x.CreatorId);

            // Relationship between category & category group.
            category.HasOne(x => x.CategoryGroup).WithMany(x => x.Categories).HasForeignKey(x => x.CategoryGroupId);
        }

        /// <summary>
        ///     Initialize topic table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void AddTopicTable(ModelBuilder modelBuilder)
        {
            // Find topic instance.
            var topic = modelBuilder.Entity<Topic>();

            // Primary key initialization.
            topic.HasKey(x => x.Id);
            topic.Property(x => x.Id).UseSqlServerIdentityColumn();

            // Relationship between topic and account.
            topic.HasOne(x => x.Owner).WithMany(x => x.Topics).HasForeignKey(x => x.OwnerId);

            // Relationship between topic and category.
            topic.HasOne(x => x.Category).WithMany(x => x.Topics).HasForeignKey(x => x.CategoryId);
        }

        /// <summary>
        ///     Initialize reply table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void AddReplyTable(ModelBuilder modelBuilder)
        {
            var reply = modelBuilder.Entity<Reply>();

            // Primary key setting.
            reply.HasKey(x => x.Id);
            reply.Property(x => x.Id).UseSqlServerIdentityColumn();

            // Relationship between reply and account.
            reply.HasOne(x => x.Owner).WithMany(x => x.Replies).HasForeignKey(x => x.OwnerId);

            // Relationship between reply and topic.
            reply.HasOne(x => x.Topic).WithMany(x => x.Replies).HasForeignKey(x => x.TopicId);
        }

        /// <summary>
        ///     Initialize activation token table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void AddActivationTokenTable(ModelBuilder modelBuilder)
        {
            var activationToken = modelBuilder.Entity<ActivationToken>();

            // Primary key setting.
            activationToken.HasKey(x => new {x.Code, x.OwnerId});
            activationToken.Property(x => x.Code).IsRequired();

            // Relationship between reply and topic.
            activationToken.HasOne(x => x.Owner).WithOne(x => x.ActivationToken)
                .HasForeignKey<ActivationToken>(x => x.OwnerId);
        }

        /// <summary>
        ///     Add access token table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void AddAccessTokenTable(ModelBuilder modelBuilder)
        {
            var accessToken = modelBuilder.Entity<AccessToken>();
            accessToken.HasKey(x => new {x.Code, x.OwnerId});

            accessToken.HasOne(x => x.Owner).WithMany(x => x.AccessTokens).HasForeignKey(x => x.OwnerId);
        }

        /// <summary>
        ///     Initialize follow category table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void AddFollowCategoryTable(ModelBuilder modelBuilder)
        {
            // Find follow category instance.
            var followCategory = modelBuilder.Entity<FollowCategory>();

            // Primary key initialization.
            followCategory.HasKey(x => new {x.FollowerId, x.CategoryId});

            // Relationship between follow category and account.
            followCategory.HasOne(x => x.Follower).WithMany(x => x.FollowCategories).HasForeignKey(x => x.FollowerId);

            // Relationship between follow category and category.
            followCategory.HasOne(x => x.Category).WithMany(x => x.FollowCategories).HasForeignKey(x => x.CategoryId);
        }

        /// <summary>
        ///     Initialize follow post table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void AddFollowTopicTable(ModelBuilder modelBuilder)
        {
            // Find follow topic instance.
            var followTopic = modelBuilder.Entity<FollowTopic>();

            // Primary key initialization.
            followTopic.HasKey(x => new {x.FollowerId, x.TopicId});

            // Relationship between follow topic & topic
            followTopic.HasOne(x => x.Topic).WithMany(x => x.FollowTopics).HasForeignKey(x => x.TopicId);

            // Relationship between follower & user..
            followTopic.HasOne(x => x.Follower).WithMany(x => x.FollowTopics).HasForeignKey(x => x.FollowerId);
        }

        /// <summary>
        ///     Initialize post report table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void AddReportTopicTable(ModelBuilder modelBuilder)
        {
            // Find post report instance.
            var topicReport = modelBuilder.Entity<ReportTopic>();

            // Primary key initialization.
            topicReport.HasKey(x => new {x.TopicId, x.ReporterId});
            topicReport.Property(x => x.Reason).IsRequired();

            // Relationship between topic report and topic.
            topicReport.HasOne(x => x.Topic).WithMany(x => x.ReportTopics).HasForeignKey(x => x.TopicId);

            // Relationship between topic report and account.
            topicReport.HasOne(x => x.TopicOwner).WithMany(x => x.OwnedTopicReports).HasForeignKey(x => x.OwnerId);
            topicReport.HasOne(x => x.TopicReporter).WithMany(x => x.ReportedPosts).HasForeignKey(x => x.ReporterId);
        }

        /// <summary>
        ///     Add signalr connection table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void AddSignalrConnectionTable(ModelBuilder modelBuilder)
        {
            var signalrConnection = modelBuilder.Entity<SignalrConnection>();
            signalrConnection.HasKey(x => x.ClientId);

            signalrConnection.Property(x => x.ClientId).IsRequired();
        }

        /// <summary>
        ///     Initialize notification message table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void AddNotificationMessageTable(ModelBuilder modelBuilder)
        {
            // Find notification message.
            var notificationMessage = modelBuilder.Entity<NotificationMessage>();

            // Primary key initialization.
            notificationMessage.HasKey(x => x.Id);
            notificationMessage.Property(x => x.Id);

            // Relationship between notification message and account.
            notificationMessage.HasOne(x => x.Owner).WithMany(x => x.NotificationMessages);
        }

        /// <summary>
        ///     Add signalrl connection group table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void AddUserDeviceGroupTable(ModelBuilder modelBuilder)
        {
            //var signalrConnectionGroup = modelBuilder.Entity<SignalrConnectionGroup>();
            //signalrConnectionGroup.HasKey(x => x.Id);
            var userDeviceGroup = modelBuilder.Entity<UserRealTimeGroup>();
            userDeviceGroup.HasKey(x => x.Id);
            userDeviceGroup.Property(x => x.Group).IsRequired();
        }

        /// <summary>
        ///     Add cloud messaging device group table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void AddCloudMessagingDeviceGroupTable(ModelBuilder modelBuilder)
        {
            var userDeviceToken = modelBuilder.Entity<UserDeviceToken>();
            userDeviceToken.HasKey(x => x.DeviceId);
            userDeviceToken.Property(x => x.DeviceId).IsRequired();

            userDeviceToken.HasOne(x => x.User).WithMany(x => x.DeviceTokens).HasForeignKey(x => x.UserId);
        }

        /// <summary>
        /// Add category summary table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void AddCategorySummaryTable(ModelBuilder modelBuilder)
        {
            var categorySummary = modelBuilder.Entity<CategorySummary>();
            categorySummary.HasKey(x => x.CategoryId);

            categorySummary.HasOne(x => x.Category).WithOne(x => x.CategorySummary)
                .HasForeignKey<CategorySummary>(x => x.CategoryId);

            categorySummary.HasOne(x => x.LastTopic).WithOne(x => x.CategorySummary)
                .HasForeignKey<CategorySummary>(x => x.LastTopicId);
        }

        /// <summary>
        /// Add topic summary table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void AddTopicSummaryTable(ModelBuilder modelBuilder)
        {
            var topicSummary = modelBuilder.Entity<TopicSummary>();
            topicSummary.HasKey(x => x.TopicId);

            topicSummary.HasOne(x => x.Topic).WithOne(x => x.TopicSummary).HasForeignKey<TopicSummary>(x => x.TopicId);
        }

        #endregion

        #endregion
    }
}