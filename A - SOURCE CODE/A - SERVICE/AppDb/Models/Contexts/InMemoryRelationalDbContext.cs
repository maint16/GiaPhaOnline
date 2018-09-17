using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
            base.OnModelCreating(modelBuilder);
            foreach (var type in _dbSeedOption.Columns.Keys)
            {
                var originalContent = _dbSeedOption.Columns[type];
                var i = Activator.CreateInstance(typeof(List<>).MakeGenericType(type)) as IList;
                var originalEntities = JsonConvert.DeserializeAnonymousType(originalContent, i);
                modelBuilder.Entity(type).HasData(originalEntities);
                
            }
        }
        
        #endregion
    }
}