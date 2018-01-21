using SystemDatabase.Interfaces.Repositories;
using SystemDatabase.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace SystemDatabase.Repositories
{
    public class RepositoryToken : ParentRepository<Token>, IRepositoryToken
    {
        #region Constructors

        /// <summary>
        ///     Initiate repository with dependency injection.
        /// </summary>
        /// <param name="dbContext"></param>
        public RepositoryToken(
            DbContext dbContext) : base(dbContext)
        {
        }

        #endregion
    }
}