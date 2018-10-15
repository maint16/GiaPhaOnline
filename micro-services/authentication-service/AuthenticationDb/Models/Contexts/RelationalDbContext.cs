using System.Linq;
using AuthenticationDb.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AuthenticationDb.Models.Contexts
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

        #endregion

        #region Methods

        /// <summary>
        ///     Callback which is fired when model is being created.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User table.
            AddUserTable(modelBuilder);

            // Use model builder to specify composite primary keys.
            // Composite primary keys configuration

            // This is for remove pluralization naming convention in database defined by Entity Framework.
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
                entity.Relational().TableName = entity.DisplayName();

            // Disable cascade delete.
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }

        #endregion

        #region Tables initialization

        /// <summary>
        /// Initialize account table.
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void AddUserTable(ModelBuilder modelBuilder)
        {
            var user = modelBuilder.Entity<User>();

            // Set primary key.
            user.HasKey(x => x.Id);
            user.Property(x => x.Id).UseSqlServerIdentityColumn();

            user.Property(x => x.Email).IsRequired();
            user.Property(x => x.Nickname).IsRequired();
        }

        #endregion

    }
}
