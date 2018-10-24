using System.Threading;
using System.Threading.Tasks;
using AuthenticationDb.Models.Entities;
using AuthenticationShared.ViewModels.Jwt;
using AuthenticationShared.ViewModels.User;

namespace AuthenticationBusiness.Interfaces.Domains
{
    public interface IUserDomain
    {
        #region Methods

        /// <summary>
        ///     Find user login information in system.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<User> LoginAsync(LoginViewModel model, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Exchange google identity with user information.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<User> GoogleLoginAsync(GoogleLoginViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Exchange facebook identity with user information.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<User> FacebookLoginAsync(FacebookLoginViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     From user information to generate user json web token.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        JwtViewModel GenerateJwt(User user);

        #endregion
    }
}