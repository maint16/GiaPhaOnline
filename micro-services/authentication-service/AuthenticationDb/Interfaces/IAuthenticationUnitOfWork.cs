using AuthenticationDb.Models.Entities;
using ServiceShared.Interfaces.Services;

namespace AuthenticationDb.Interfaces
{
    public interface IAuthenticationUnitOfWork : IBaseUnitOfWork
    {
        #region Properties

        /// <summary>
        ///     User repository in the system.
        /// </summary>
        IBaseRepository<User> Users { get; set; }

        #endregion
    }
}