using AuthenticationDb.Interfaces;
using AuthenticationDb.Models.Entities;
using Microsoft.EntityFrameworkCore;
using ServiceShared.Interfaces.Services;
using ServiceShared.Services;

namespace AuthenticationDb.Services
{
    public class AuthenticationUnitOfWork : BaseUnitOfWork, IAuthenticationUnitOfWork
    {
        #region Constructors

        /// <summary>
        ///     Initiate unit of work with database context provided by Entity Framework.
        /// </summary>
        public AuthenticationUnitOfWork(DbContext dbContext, IRepository<User> users) : base(dbContext)
        {
            Users = users;
        }

        #endregion

        #region Properties

        public IRepository<User> Users { get; set; }

        #endregion
    }
}