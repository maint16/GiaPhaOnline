using System.Threading;
using System.Threading.Tasks;
using AuthenticationDb.Models.Entities;
using AuthenticationMicroService.ViewModels.User;

namespace AuthenticationMicroService.Interfaces.Services.Businesses
{
    public interface IUserService
    {
        #region Methods

        /// <summary>
        /// Find user login information in system.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<User> LoginAsync(LoginViewModel model, CancellationToken cancellationToken = default(CancellationToken));

        #endregion
    }
}
