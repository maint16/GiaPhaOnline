using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using SystemDatabase.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace SystemDatabase.Models.Contexts
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
        ///     List of categories in database.
        /// </summary>
        public virtual DbSet<Category> Categories { get; set; }

        /// <summary>
        /// List of categorizations.
        /// </summary>
        public virtual DbSet<Categorization> Categorizations { get; set; }

        /// <summary>
        ///     List of comments in database.
        /// </summary>
        public virtual DbSet<Comment> Comments { get; set; }

        /// <summary>
        ///     List of comment reports in database.
        /// </summary>
        public virtual DbSet<CommentReport> CommentReports { get; set; }

        /// <summary>
        ///     List of relationships between followers and categories. (many - many)
        /// </summary>
        public virtual DbSet<FollowCategory> FollowCategories { get; set; }

        /// <summary>
        ///     List of relationships between followers and posts (many - many).
        /// </summary>
        public virtual DbSet<FollowPost> FollowPosts { get; set; }

        /// <summary>
        ///     List of comment notifications.
        /// </summary>
        public virtual DbSet<CommentNotification> CommentNotifications { get; set; }

        /// <summary>
        ///     List of post notifications.
        /// </summary>
        public virtual DbSet<PostNotification> PostNotifications { get; set; }

        /// <summary>
        ///     List of posts.
        /// </summary>
        public virtual DbSet<Post> Posts { get; set; }

        /// <summary>
        ///     List of post reports.
        /// </summary>
        public virtual DbSet<PostReport> PostReports { get; set; }

        /// <summary>
        ///     List of tokens in database.
        /// </summary>
        public virtual DbSet<Token> Tokens { get; set; }

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
            InitializeCategory(modelBuilder);
            InitializeCategorization(modelBuilder);
            InitializeComment(modelBuilder);
            InitializeCommentNotification(modelBuilder);
            InitializeCommentReport(modelBuilder);
            InitializeFollowCategory(modelBuilder);
            InitializeFollowPost(modelBuilder);
            InitializePost(modelBuilder);
            InitializePostNotification(modelBuilder);
            InitializePostReport(modelBuilder);
            InitializeSignalrConnection(modelBuilder);
            InitializeToken(modelBuilder);

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
        private void InitializeCategory(ModelBuilder modelBuilder)
        {
            var category = modelBuilder.Entity<Category>();

            // Set primary key.
            category.HasKey(x => x.Id);
            category.Property(x => x.Id).UseSqlServerIdentityColumn();

            // Relationship between category & account.
            category.HasOne(x => x.Creator).WithMany(x => x.InitializedCategories);
        }

        /// <summary>
        /// Initialize categorization table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void InitializeCategorization(ModelBuilder modelBuilder)
        {
            var categorization = modelBuilder.Entity<Categorization>();

            // Primary key setup.
            categorization.HasKey(x => new {x.PostId, x.CategoryId});

            // Relationship between categorization & category.
            categorization.HasOne(x => x.Category).WithMany(x => x.Categorizations);
            
            // Relationship between cagorization & post.
            categorization.HasOne(x => x.Post).WithMany(x => x.Categorizations);
        }

        /// <summary>
        /// Initialize comment table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void InitializeComment(ModelBuilder modelBuilder)
        {
            var comment = modelBuilder.Entity<Comment>();

            // Primary key setting.
            comment.HasKey(x => x.Id);
            comment.Property(x => x.Id).UseSqlServerIdentityColumn();

            // Relationship between comment and account.
            comment.HasOne(x => x.Owner).WithMany(x => x.Comments);

            // Relationship between comment and post,
            comment.HasOne(x => x.Post).WithMany(x => x.Comments);

            // Relationship between comment and comment notifications.
            comment.HasMany(x => x.CommentNotifications).WithOne(x => x.Comment);
        }

        /// <summary>
        /// Initialize comment notification table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void InitializeCommentNotification(ModelBuilder modelBuilder)
        {
            var commentNotification = modelBuilder.Entity<CommentNotification>();

            // Primary key setting.
            commentNotification.HasKey(x => x.Id);
            commentNotification.Property(x => x.Id).ValueGeneratedOnAdd();

            // Relationship between comment notification & comment.
            commentNotification.HasOne(x => x.Comment).WithMany(x => x.CommentNotifications);

            // Relationship between comment notification & post.
            commentNotification.HasOne(x => x.Post).WithMany(x => x.CommentNotifications);

            // Relationship between comment notification & account.
            commentNotification.HasOne(x => x.Recipient).WithMany(x => x.ReceivedCommentNotifications);
            commentNotification.HasOne(x => x.Broadcaster).WithMany(x => x.BroadcastedCommentNotifications);
        }

        /// <summary>
        /// Initialize comment report table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void InitializeCommentReport(ModelBuilder modelBuilder)
        {
            // Find comment report instance.
            var commentReport = modelBuilder.Entity<CommentReport>();

            // Primary key setting.
            commentReport.HasKey(x => new {x.CommentId, x.OwnerId});

            // Relationship between comment report & comment.
            commentReport.HasOne(x => x.Comment).WithMany(x => x.CommentReports);

            // Relationship between comment report & owner.
            commentReport.HasOne(x => x.CommentReporter).WithMany(x => x.ReportedComments);
            commentReport.HasOne(x => x.CommentOwner).WithMany(x => x.OwnedCommentReports);

            // Relationship between comment report & post.
            commentReport.HasOne(x => x.Post).WithMany(x => x.ReportedComments);
        }

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
            // Find follow post instance.
            var followPost = modelBuilder.Entity<FollowPost>();

            // Primary key initialization.
            followPost.HasKey(x => new {x.FollowerId, x.PostId});

            // Relationship between follow post and post.
            followPost.HasOne(x => x.Post).WithMany(x => x.FollowPosts);

            // Relationship between follow post and account.
            followPost.HasOne(x => x.Follower).WithMany(x => x.FollowPosts);
        }

        /// <summary>
        /// Initialize post table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void InitializePost(ModelBuilder modelBuilder)
        {
            // Find post instance.
            var post = modelBuilder.Entity<Post>();

            // Primary key initialization.
            post.HasKey(x => x.Id);
            post.Property(x => x.Id).UseSqlServerIdentityColumn();

            // Relationship between post and account.
            post.HasOne(x => x.Owner).WithMany(x => x.Posts);
        }

        /// <summary>
        /// Initialize post notification table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void InitializePostNotification(ModelBuilder modelBuilder)
        {
            // Find post notification.
            var postNotification = modelBuilder.Entity<PostNotification>();

            // Primary key initialization.
            postNotification.HasKey(x => x.Id);
            postNotification.Property(x => x.Id).ValueGeneratedOnAdd();

            // Relationship between post notification and post.
            postNotification.HasOne(x => x.Post).WithMany(x => x.PostNotifications);

            // Relationship between post notification and account.
            postNotification.HasOne(x => x.Recipient).WithMany(x => x.ReceivedPostNotifications);
            postNotification.HasOne(x => x.Broadcaster).WithMany(x => x.BroadcastedPostNotifications);
        }

        /// <summary>
        /// Initialize post report table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void InitializePostReport(ModelBuilder modelBuilder)
        {
            // Find post report instance.
            var postReport = modelBuilder.Entity<PostReport>();
            
            // Primary key initialization.
            postReport.HasKey(x => new {x.PostId, x.ReporterId});

            // Relationship between post report and account.
            postReport.HasOne(x => x.PostOwner).WithMany(x => x.OwnedPostReports);
            postReport.HasOne(x => x.PostReporter).WithMany(x => x.ReportedPosts);

        }

        /// <summary>
        /// Initialize signalr connection table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void InitializeSignalrConnection(ModelBuilder modelBuilder)
        {
            // Find signalr connection.
            var signalrConnection = modelBuilder.Entity<SignalrConnection>();

            // Primary key initialization.
            signalrConnection.HasKey(x => x.Id);
            signalrConnection.Property(x => x.Id).IsRequired();

            // Relationship between signalr connection and account.
            signalrConnection.HasOne(x => x.Owner).WithMany(x => x.SignalrConnections);
        }

        /// <summary>
        /// Initialize token table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void InitializeToken(ModelBuilder modelBuilder)
        {
            // Find token.
            var token = modelBuilder.Entity<Token>();

            // Primary key initialization.
            token.HasKey(x => x.Id);
            token.Property(x => x.Id).UseSqlServerIdentityColumn();

            // Relationship between token and account.
            token.HasOne(x => x.Owner).WithMany(x => x.Tokens);
        }

        #endregion

        #endregion
    }
}