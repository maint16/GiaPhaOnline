using SystemDatabase.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Repositories;

namespace Shared.Repositories
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