using SystemDatabase.Interfaces.Repositories;
using SystemDatabase.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace SystemDatabase.Repositories
{
    public class RepositoryCategorization : ParentRepository<Categorization>, IRepositoryCategorization
    {
        #region Constructors

        /// <summary>
        ///     Initialize repository with database context.
        /// </summary>
        /// <param name="dbContext"></param>
        public RepositoryCategorization(DbContext dbContext) : base(dbContext)
        {
        }

        #endregion
    }
}