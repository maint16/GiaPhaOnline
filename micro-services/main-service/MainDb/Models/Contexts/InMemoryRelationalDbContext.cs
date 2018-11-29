using System;
using System.Collections.Generic;
using ClientShared.Enumerations;
using MainDb.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MainDb.Models.Contexts
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

        
        #endregion
    }
}