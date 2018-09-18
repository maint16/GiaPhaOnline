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
        
        #endregion
    }
}