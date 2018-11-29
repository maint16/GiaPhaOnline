using MainDb.Models.Entities;
using ServiceShared.Interfaces.Services;

namespace MainDb.Interfaces
{
    public interface IAppUnitOfWork : IBaseUnitOfWork
    {
        #region Properties

        IBaseRepository<User> Accounts { get; }
        

        #endregion
    }
}