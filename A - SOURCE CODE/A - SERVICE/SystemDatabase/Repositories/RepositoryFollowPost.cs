using SystemDatabase.Interfaces.Repositories;
using SystemDatabase.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace SystemDatabase.Repositories
{
    public class RepositoryFollowPost : ParentRepository<FollowPost>, IRepositoryFollowPost
    {
        #region Constructor

        /// <summary>
        /// Initialize repository with injector.
        /// </summary>
        /// <param name="dbContext"></param>
        public RepositoryFollowPost(DbContext dbContext) : base(dbContext)
        {
        }

        #endregion
    }
}