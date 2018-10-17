using System;
using System.Collections.Generic;
using AppDb.Models.Entities;
using AppShared.Enumerations;
using Microsoft.EntityFrameworkCore;

namespace AppDb.Models.Contexts
{
    public class InMemoryRelationalDbContext : RelationalDbContext
    {
        #region Constructor

        /// <summary>
        ///     Initialize in-memory relational database context with injectors.
        /// </summary>
        /// <param name="dbContextOptions"></param>
        public InMemoryRelationalDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Call the base function.
            base.OnModelCreating(modelBuilder);

            // Add user mocking data.
            AddUsers(modelBuilder);

            // Add category group moking data.
            AddCategoryGroups(modelBuilder);

            // Add category moking data.
            AddCategories(modelBuilder);

            // Add topic moking data.
            AddTopics(modelBuilder);

            // Add reply mocking data.
            AddReplies(modelBuilder);

            // Add category summary mocking data.
            AddCategorySummary(modelBuilder);

            // Add topic summary.
            AddTopicSummary(modelBuilder);

            AddUserRealTimeGroup(modelBuilder);
        }

        /// <summary>
        ///     Add user mocking data.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void AddUsers(ModelBuilder modelBuilder)
        {
            // Password: abcde12345-
            var users = new List<User>();
            users.Add(new User(1, "admin@gmail.com", "Linh Nguyen", "5773961b8fb0e85fa14aec3681647c7d", UserKind.Basic,
                UserStatus.Available, UserRole.Admin, "https://via.placeholder.com/512x512", "Hello admin", 0, null));
            users.Add(new User(2, "user@gmail.com", "Linh Tran", "5773961b8fb0e85fa14aec3681647c7d", UserKind.Basic,
                UserStatus.Available, UserRole.User, "https://via.placeholder.com/512x512", "Hello user", 0, null));
            users.Add(new User(3, "lightalakanzam@gmail.com", "Admin", "5773961b8fb0e85fa14aec3681647c7d",
                UserKind.Google, UserStatus.Available, UserRole.User, "https://via.placeholder.com/512x512",
                "Hello user", 0, null));

            modelBuilder.Entity<User>()
                .HasData(users.ToArray());
        }

        /// <summary>
        ///     Add category group mocking data.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void AddCategoryGroups(ModelBuilder modelBuilder)
        {
            var categoryGroups = new List<CategoryGroup>();
            categoryGroups.Add(new CategoryGroup(1, 1, "16+", "16+", ItemStatus.Active, 0, null));
            categoryGroups.Add(new CategoryGroup(2, 1, "18+", "18+", ItemStatus.Active, 0, null));

            modelBuilder.Entity<CategoryGroup>()
                .HasData(categoryGroups.ToArray());
        }

        /// <summary>
        ///     Add category moking data.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void AddCategories(ModelBuilder modelBuilder)
        {
            var categories = new List<Category>();
            categories.Add(new Category(1, 1, 1, "https://via.placeholder.com/512x512", "18+", ItemStatus.Active, "18+",
                0, null));
            categories.Add(new Category(2, 1, 1, "https://via.placeholder.com/512x512", "16+", ItemStatus.Active, "18+",
                0, null));

            modelBuilder.Entity<Category>().HasData(categories.ToArray());
        }

        /// <summary>
        ///     Add topic moking data.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void AddTopics(ModelBuilder modelBuilder)
        {
            var topics = new List<Topic>();
            topics.Add(new Topic(1, 1, 1, 1, "Có nên mua mibook air ko các bác", "Mấy con 12.5 13.3 của nó ngon ko",
                TopicType.Public,
                ItemStatus.Active, 0, null));
            topics.Add(new Topic(2, 2, 1, 1, "Mifit ray sale còn 10$ ", "Mua 10 cái còn 35$", 0, ItemStatus.Active, 0,
                null));

            modelBuilder.Entity<Topic>().HasData(topics.ToArray());
        }

        /// <summary>
        ///     Add reply moking data.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void AddReplies(ModelBuilder modelBuilder)
        {
            var replies = new List<Reply>();
            replies.Add(new Reply(1, 1, 1, 1, 1,
                "hôm trước ra mi store thấy rồi, build khá tốt đó, văn phòng web này nọ thì đượ", ItemStatus.Active, 0,
                null));
            replies.Add(new Reply(2, 2, 1, 1, 1, "cho em ké với thím", ItemStatus.Disabled, 0, null));

            modelBuilder.Entity<Reply>().HasData(replies.ToArray());
        }

        /// <summary>
        ///     Add user real-time group.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void AddUserRealTimeGroup(ModelBuilder modelBuilder)
        {
            var userRealTimeGroups = new List<UserRealTimeGroup>();
            userRealTimeGroups.Add(new UserRealTimeGroup(Guid.NewGuid(), "Admin", 1, 0));
            modelBuilder.Entity<UserRealTimeGroup>().HasData(userRealTimeGroups.ToArray());
        }

        /// <summary>
        /// Add category summary.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void AddCategorySummary(ModelBuilder modelBuilder)
        {
            var categorySummaries = new List<CategorySummary>();
            categorySummaries.Add(new CategorySummary(1, 2, 0, 2, "Mifit ray sale còn 10$", 0));

            modelBuilder.Entity<CategorySummary>().HasData(categorySummaries.ToArray());
        }

        /// <summary>
        /// Add topic summary.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected virtual void AddTopicSummary(ModelBuilder modelBuilder)
        {
            var topicSummaries = new List<TopicSummary>();
            topicSummaries.Add(new TopicSummary(1, 0, 2));

            modelBuilder.Entity<TopicSummary>().HasData(topicSummaries.ToArray());
        }

        #endregion
    }
}