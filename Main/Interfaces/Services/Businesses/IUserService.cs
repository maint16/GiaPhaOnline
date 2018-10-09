using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppDb.Models.Entities;
using Main.Models;
using Main.ViewModels.Users;
using Shared.Models;

namespace Main.Interfaces.Services.Businesses
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

        /// <summary>
        /// Exchange google identity with user information.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<User> GoogleLoginAsync(GoogleLoginViewModel model, CancellationToken cancellationToken = default (CancellationToken));

        /// <summary>
        /// Exchange facebook identity with user information.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<User> FacebookLoginAsync(FacebookLoginViewModel model, CancellationToken cancellationToken = default (CancellationToken));

        /// <summary>
        /// Register user information basically.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<User> BasicRegisterAsync(RegisterAccountViewModel model, CancellationToken cancellationToken = default (CancellationToken));

        /// <summary>
        /// Search for users using specific conditions.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SearchResult<IList<User>>> SearchUsersAsync(SearchUserViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Search for an user with specific conditions.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<User> SearchUserAsync(SearchUserViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Forgot password asynchronously.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ForgotPasswordResultModel> RequestPasswordResetAsync(ForgotPasswordViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        #endregion
    }
}