using MainDb.Interfaces;
using MainDb.Models.Entities;
using Microsoft.EntityFrameworkCore;
using ServiceShared.Interfaces.Services;
using ServiceShared.Services;

namespace MainDb.Services
{
    public class AppUnitOfWork : BaseUnitOfWork, IAppUnitOfWork
    {
        #region Constructors

        /// <summary>
        ///     Initiate unit of work with database context provided by Entity Framework.
        /// </summary>
        public AppUnitOfWork(DbContext dbContext, IBaseRepository<User> accounts
         ) : base(dbContext)
        {
            Accounts = accounts;
            
        }

        #endregion


        #region Properties

        public IBaseRepository<User> Accounts { get; }
        

        #endregion
    }
}