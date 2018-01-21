using SystemDatabase.Interfaces.Repositories;
using SystemDatabase.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace SystemDatabase.Repositories
{
    public class RepositorySignalrConnection : ParentRepository<SignalrConnection>, IRepositorySignalrConnection
    {
        #region Constructors

        public RepositorySignalrConnection(DbContext dbContext) : base(dbContext)
        {
        }

        #endregion
    }
}