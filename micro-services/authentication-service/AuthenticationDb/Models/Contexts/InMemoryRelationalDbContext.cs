using System.Collections.Generic;
using AuthenticationDb.Models.Entities;
using ClientShared.Enumerations;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationDb.Models.Contexts
{
    public class InMemoryRelationalDbContext : RelationalDbContext
    {
        #region Properties

        private readonly DbSeedOption _dbSeedOption;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initialize in-memory relational database context with injectors.
        /// </summary>
        /// <param name="dbContextOptions"></param>
        /// <param name="dbSeedOptions"></param>
        public InMemoryRelationalDbContext(DbContextOptions dbContextOptions, DbSeedOption dbSeedOptions) : base(
            dbContextOptions)
        {
            _dbSeedOption = dbSeedOptions;
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
        }

        /// <summary>
        ///     Add user mocking data.
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void AddUsers(ModelBuilder modelBuilder)
        {
            var users = new List<User>();
            users.Add(new User(1, "admin@gmail.com", "Admin", "5773961b8fb0e85fa14aec3681647c7d",
                "https://via.placeholder.com/512x512", UserStatus.Available, UserKind.Basic, UserRole.Admin,
                "Hello admin", 0, null));
            users.Add(new User(2, "user@gmail.com", "Admin", "5773961b8fb0e85fa14aec3681647c7d",
                "https://via.placeholder.com/512x512", UserStatus.Available, UserKind.Basic, UserRole.User,
                "Hello user", 0, null));

            modelBuilder.Entity<User>()
                .HasData(users.ToArray());
        }

        #endregion
    }
}