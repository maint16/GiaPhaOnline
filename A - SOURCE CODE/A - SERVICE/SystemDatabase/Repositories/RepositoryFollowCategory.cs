using SystemDatabase.Interfaces.Repositories;
using SystemDatabase.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace SystemDatabase.Repositories
{
    public class RepositoryFollowCategory : ParentRepository<FollowCategory>, IRepositoryFollowCategory
    {
        #region Constructors

        /// <summary>
        /// Initialize repository with injector
        /// </summary>
        /// <param name="dbContext"></param>
        public RepositoryFollowCategory(DbContext dbContext) : base(dbContext)
        {
        }

        #endregion
    }
}