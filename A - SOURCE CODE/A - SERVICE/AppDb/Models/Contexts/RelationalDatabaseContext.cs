using System.Linq;
using System.Threading.Tasks;
using AppDb.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AppDb.Models.Contexts
{
    public class RelationalDatabaseContext : DbContext
    {
        #region Constructors

        /// <summary>
        ///     Initiate database context with connection string.
        /// </summary>
        public RelationalDatabaseContext(DbContextOptions<RelationalDatabaseContext> dbContextOptions)
            : base(dbContextOptions)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///     List of accounts in database.
        /// </summary>
        public virtual DbSet<Account> Accounts { get; set; }

        /// <summary>
        ///     List of category groups in database.
        /// </summary>
        public virtual DbSet<CategoryGroup> CategoryGroups { get; set; }

        /// <summary>
        ///     List of categories in database.
        /// </summary>
        public virtual DbSet<Category> Categories { get; set; }

        /// <summary>
        ///     List of comments in database.
        /// </summary>
        public virtual DbSet<Reply> Comments { get; set; }

        /// <summary>
        ///     List of relationships between followers and categories. (many - many)
        /// </summary>
        public virtual DbSet<FollowCategory> FollowCategories { get; set; }

        /// <summary>
        ///     List of relationships between followers and posts (many - many).
        /// </summary>
        public virtual DbSet<FollowTopic> FollowPosts { get; set; }
        
        /// <summary>
        ///     List of posts.
        /// </summary>
        public virtual DbSet<Topic> Posts { get; set; }

        /// <summary>
        ///     List of post reports.
        /// </summary>
        public virtual DbSet<ReportTopic> PostReports { get; set; }

        /// <summary>
        ///     List of tokens in database.
        /// </summary>
        public virtual DbSet<AccessToken> Tokens { get; set; }

        /// <summary>
        ///     List of notification messages in database.
        /// </summary>
        public virtual DbSet<NotificationMessage> NotificationMessages { get; set; }

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
            // Tables initialization.
            InitializeAccount(modelBuilder);
            InitializeCategoryGroup(modelBuilder);
            InitializeCategory(modelBuilder);
            InitializeComment(modelBuilder);
            InitializeFollowCategory(modelBuilder);
            InitializeFollowPost(modelBuilder);
            InitializePost(modelBuilder);
            InitializePostReport(modelBuilder);
            InitializeToken(modelBuilder);
            InitializeNotificationMessage(modelBuilder);

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
        /// Initialize account table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void InitializeAccount(ModelBuilder modelBuilder)
        {
            var account = modelBuilder.Entity<Account>();

            // Set primary key.
            account.HasKey(x => x.Id);
            account.Property(x => x.Id).UseSqlServerIdentityColumn();
        }

        /// <summary>
        /// Initialize category table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void InitializeCategoryGroup(ModelBuilder modelBuilder)
        {
            var categoryGroup = modelBuilder.Entity<CategoryGroup>();

            // Set primary key.
            categoryGroup.HasKey(x => x.Id);
            categoryGroup.Property(x => x.Id).UseSqlServerIdentityColumn();

            // Relationship between category group & account.
            categoryGroup.HasOne(x => x.Creator).WithMany(x => x.CategoryGroups);
        }

        /// <summary>
        /// Initialize category table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void InitializeCategory(ModelBuilder modelBuilder)
        {
            var category = modelBuilder.Entity<Category>();

            // Set primary key.
            category.HasKey(x => x.Id);
            category.Property(x => x.Id).UseSqlServerIdentityColumn();

            // Relationship between category & account.
            category.HasOne(x => x.Creator).WithMany(x => x.Categories);

            // Relationship between category & category group.
            category.HasOne(x => x.CategoryGroup).WithMany(x => x.Categories);
        }

        /// <summary>
        /// Initialize reply table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void InitializeComment(ModelBuilder modelBuilder)
        {
            var reply = modelBuilder.Entity<Reply>();

            // Primary key setting.
            reply.HasKey(x => x.Id);
            reply.Property(x => x.Id).UseSqlServerIdentityColumn();

            // Relationship between reply and account.
            reply.HasOne(x => x.Owner).WithMany(x => x.Replies);

            // Relationship between reply and topic.
            reply.HasOne(x => x.Topic).WithMany(x => x.Replies);
        }

//        /// <summary>
//        /// Initialize comment notification table.
//        /// </summary>
//        /// <param name="modelBuilder"></param>
//        private void InitializeCommentNotification(ModelBuilder modelBuilder)
//        {
//            var commentNotification = modelBuilder.Entity<CommentNotification>();
//
//            // Primary key setting.
//            commentNotification.HasKey(x => x.Id);
//            commentNotification.Property(x => x.Id).ValueGeneratedOnAdd();
//
//            // Relationship between comment notification & comment.
//            commentNotification.HasOne(x => x.Comment).WithMany(x => x.CommentNotifications);
//
//            // Relationship between comment notification & post.
//            commentNotification.HasOne(x => x.Post).WithMany(x => x.CommentNotifications);
//
//            // Relationship between comment notification & account.
//            commentNotification.HasOne(x => x.Recipient).WithMany(x => x.ReceivedCommentNotifications);
//            commentNotification.HasOne(x => x.Broadcaster).WithMany(x => x.BroadcastedCommentNotifications);
//        }

//        /// <summary>
//        /// Initialize comment report table.
//        /// </summary>
//        /// <param name="modelBuilder"></param>
//        private void InitializeCommentReport(ModelBuilder modelBuilder)
//        {
//            // Find comment report instance.
//            var commentReport = modelBuilder.Entity<CommentReport>();
//
//            // Primary key setting.
//            commentReport.HasKey(x => new {x.CommentId, x.OwnerId});
//
//            // Relationship between comment report & comment.
//            commentReport.HasOne(x => x.Comment).WithMany(x => x.CommentReports);
//
//            // Relationship between comment report & owner.
//            commentReport.HasOne(x => x.CommentReporter).WithMany(x => x.ReportedComments);
//            commentReport.HasOne(x => x.CommentOwner).WithMany(x => x.OwnedCommentReports);
//
//            // Relationship between comment report & post.
//            commentReport.HasOne(x => x.Post).WithMany(x => x.ReportedComments);
//        }

        /// <summary>
        /// Initialize follow category table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void InitializeFollowCategory(ModelBuilder modelBuilder)
        {
            // Find follow category instance.
            var followCategory = modelBuilder.Entity<FollowCategory>();

            // Primary key initialization.
            followCategory.HasKey(x => new {x.FollowerId, x.CategoryId});
            
            // Relationship between follow category and account.
            followCategory.HasOne(x => x.Follower).WithMany(x => x.FollowCategories);

            // Relationship between follow category and category.
            followCategory.HasOne(x => x.Category).WithMany(x => x.FollowCategories);
        }

        /// <summary>
        /// Initialize follow post table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void InitializeFollowPost(ModelBuilder modelBuilder)
        {
            // Find follow topic instance.
            var followTopic = modelBuilder.Entity<FollowTopic>();

            // Primary key initialization.
            followTopic.HasKey(x => new {x.FollowerId, x.TopicId});

            // Relationship between follow post and post.
            followTopic.HasOne(x => x.Topic).WithMany(x => x.FollowTopics);

            // Relationship between follow post and account.
            followTopic.HasOne(x => x.Follower).WithMany(x => x.FollowTopics);
        }

        /// <summary>
        /// Initialize post table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void InitializePost(ModelBuilder modelBuilder)
        {
            // Find topic instance.
            var topic = modelBuilder.Entity<Topic>();

            // Primary key initialization.
            topic.HasKey(x => x.Id);
            topic.Property(x => x.Id).UseSqlServerIdentityColumn();

            // Relationship between topic and account.
            topic.HasOne(x => x.Owner).WithMany(x => x.Topics);

            // Relationship between topic and category.
            topic.HasOne(x => x.Category).WithMany(x => x.Topics);
        }

//        /// <summary>
//        /// Initialize post notification table.
//        /// </summary>
//        /// <param name="modelBuilder"></param>
//        private void InitializePostNotification(ModelBuilder modelBuilder)
//        {
//            // Find post notification.
//            var postNotification = modelBuilder.Entity<PostNotification>();
//
//            // Primary key initialization.
//            postNotification.HasKey(x => x.Id);
//            postNotification.Property(x => x.Id).ValueGeneratedOnAdd();
//
//            // Relationship between post notification and post.
//            postNotification.HasOne(x => x.Post).WithMany(x => x.PostNotifications);
//
//            // Relationship between post notification and account.
//            postNotification.HasOne(x => x.Recipient).WithMany(x => x.ReceivedPostNotifications);
//            postNotification.HasOne(x => x.Broadcaster).WithMany(x => x.BroadcastedPostNotifications);
//        }

        /// <summary>
        /// Initialize post report table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void InitializePostReport(ModelBuilder modelBuilder)
        {
            // Find post report instance.
            var topicReport = modelBuilder.Entity<ReportTopic>();

            // Primary key initialization.
            topicReport.HasKey(x => new {x.TopicId, x.ReporterId});

            // Relationship between topic report and topic.
            topicReport.HasOne(x => x.Topic).WithMany(x => x.ReportTopics);

            // Relationship between topic report and account.
            topicReport.HasOne(x => x.TopicOwner).WithMany(x => x.OwnedTopicReports);
            topicReport.HasOne(x => x.TopicReporter).WithMany(x => x.ReportedPosts);

        }

//        /// <summary>
//        /// Initialize signalr connection table.
//        /// </summary>
//        /// <param name="modelBuilder"></param>
//        private void InitializeSignalrConnection(ModelBuilder modelBuilder)
//        {
//            // Find signalr connection.
//            var signalrConnection = modelBuilder.Entity<SignalrConnection>();
//
//            // Primary key initialization.
//            signalrConnection.HasKey(x => x.Id);
//            signalrConnection.Property(x => x.Id).IsRequired();
//
//            // Relationship between signalr connection and account.
//            signalrConnection.HasOne(x => x.Owner).WithMany(x => x.SignalrConnections);
//        }

        /// <summary>
        /// Initialize token table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void InitializeToken(ModelBuilder modelBuilder)
        {
            // Find token.
            var token = modelBuilder.Entity<AccessToken>();

            // Primary key initialization.
            token.HasKey(x => x.Id);
            token.Property(x => x.Id).UseSqlServerIdentityColumn();

            // Relationship between token and account.
            token.HasOne(x => x.Owner).WithMany(x => x.AccessTokens);
        }

        /// <summary>
        /// Initialize notification message table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void InitializeNotificationMessage(ModelBuilder modelBuilder)
        {
            // Find notification message.
            var notificationMessage = modelBuilder.Entity<NotificationMessage>();

            // Primary key initialization.
            notificationMessage.HasKey(x => x.Id);
            notificationMessage.Property(x => x.Id).UseSqlServerIdentityColumn();

            // Relationship between notification message and account.
            notificationMessage.HasOne(x => x.Owner).WithMany(x => x.NotificationMessages);
        }

        //        /// <summary>
        //        /// Initialize device table.
        //        /// </summary>
        //        /// <param name="modelBuilder"></param>
        //        private void InitializeDevice(ModelBuilder modelBuilder)
        //        {
        //            // Find device instance.
        //            var device = modelBuilder.Entity<Device>();
        //
        //            // Primary key initialization.
        //            device.HasKey(x => x.Id);
        //
        //            // Relationship between account & device.
        //            device.HasOne(x => x.Owner).WithMany(x => x.Devices);
        //        }

        //        /// <summary>
        //        /// Initialize device table.
        //        /// </summary>
        //        /// <param name="modelBuilder"></param>
        //        private void InitializeFcmGroup(ModelBuilder modelBuilder)
        //        {
        //            // Find device instance.
        //            var device = modelBuilder.Entity<FcmGroup>();
        //
        //            // Primary key initialization.
        //            device.HasKey(x => x.Name);
        //            device.Property(x => x.Name).IsRequired();
        //
        //            device.Property(x => x.MessagingKey).IsRequired();
        //        }

        #endregion

        #endregion
    }
}