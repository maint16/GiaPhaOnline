using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using AppDb.Models.Entities;
using AppModel.Enumerations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AppDb.Models.Contexts
{
    public class InMemoryRelationalDbContext : RelationalDbContext
    {
        #region Properties

        private readonly DbSeedOption _dbSeedOption;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize in-memory relational database context with injectors.
        /// </summary>
        /// <param name="dbContextOptions"></param>
        /// <param name="dbSeedOptions"></param>
        public InMemoryRelationalDbContext(DbContextOptions dbContextOptions, DbSeedOption dbSeedOptions) : base(dbContextOptions)
        {
            _dbSeedOption = dbSeedOptions;
        }

        #endregion

        #region Methods

        /// <summary>
        /// <inheritdoc/>
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

            // Add reply moking data.
            AddReplies(modelBuilder);

            AddUserRealTimeGroup(modelBuilder);

            //foreach (var type in _dbSeedOption.Columns.Keys)
            //{
            //    var originalContent = _dbSeedOption.Columns[type];
            //    var i = Activator.CreateInstance(typeof(List<>).MakeGenericType(type)) as IList;
            //    var originalEntities = JsonConvert.DeserializeAnonymousType(originalContent, i);
            //    modelBuilder.Entity(type).HasData(originalEntities);
            //}
        }

        /// <summary>
        /// Add user mocking data.
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void AddUsers(ModelBuilder modelBuilder)
        {

            var users = new List<User>();
            users.Add(new User(1, "admin@gmail.com", "Admin", "5773961b8fb0e85fa14aec3681647c7d", UserKind.Basic, UserStatus.Available, UserRole.Admin, "https://via.placeholder.com/512x512", "Hello admin", 0, null));
            users.Add(new User(2, "user@gmail.com", "Admin", "5773961b8fb0e85fa14aec3681647c7d", UserKind.Basic, UserStatus.Available, UserRole.User, "https://via.placeholder.com/512x512", "Hello user", 0, null));

            modelBuilder.Entity<User>()
                .HasData(users.ToArray());
        }

        /// <summary>
        /// Add category group mocking data.
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void AddCategoryGroups(ModelBuilder modelBuilder)
        {
            var categoryGroups = new List<CategoryGroup>();
            categoryGroups.Add(new CategoryGroup(1, 1, "16+", "16+", ItemStatus.Active, 0, null));
            categoryGroups.Add(new CategoryGroup(2, 1, "18+", "18+", ItemStatus.Active, 0, null));

            modelBuilder.Entity<CategoryGroup>()
                .HasData(categoryGroups.ToArray());
        }

        /// <summary>
        /// Add category moking data.
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void AddCategories(ModelBuilder modelBuilder)
        {
            var categories = new List<Category>();
            categories.Add(new Category(1, 1, 1, "https://via.placeholder.com/512x512", "18+", 0, "18+", 0, null));
            categories.Add(new Category(2, 1, 1, "https://via.placeholder.com/512x512", "16+", 0, "18+", 0, null));

            modelBuilder.Entity<Category>().HasData(categories.ToArray());
        }

        /// <summary>
        /// Add topic moking data.
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void AddTopics(ModelBuilder modelBuilder)
        {
            var topics = new List<Topic>();
            topics.Add(new Topic(1, 1, 1, 1, "Có nên mua mibook air ko các bác", "Mấy con 12.5 13.3 của nó ngon ko", 0, 0, 0, null));
            topics.Add(new Topic(2, 2, 1, 1, "Mifit ray sale còn 10$ ", "Mua 10 cái còn 35$", 0, 0, 0, null));

            modelBuilder.Entity<Topic>().HasData(topics.ToArray());
        }

        /// <summary>
        /// Add reply moking data.
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void AddReplies(ModelBuilder modelBuilder)
        {
            var replies = new List<Reply>();
            replies.Add(new Reply(1, 1, 1, 1, 1, "hôm trước ra mi store thấy rồi, build khá tốt đó, văn phòng web này nọ thì đượ", 0, 0, null));
            replies.Add(new Reply(2, 2, 1, 1, 1, "cho em ké với thím", 0, 0, null));

            modelBuilder.Entity<Reply>().HasData(replies.ToArray());
        }

        /// <summary>
        /// Add user real-time group.
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void AddUserRealTimeGroup(ModelBuilder modelBuilder)
        {
            var userRealTimeGroups = new List<UserRealTimeGroup>();
            userRealTimeGroups.Add(new UserRealTimeGroup(Guid.NewGuid(), "Admin", 1, 0));
            modelBuilder.Entity<UserRealTimeGroup>().HasData(userRealTimeGroups.ToArray());
        }
        #endregion
    }
}