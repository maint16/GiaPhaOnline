using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClientShared.Models;
using MainBusiness.Models.Users;
using MainDb.Models.Entities;
using MainShared.ViewModels.Jwt;
using MainShared.ViewModels.Users;
using SkiaSharp;

namespace MainBusiness.Interfaces.Domains
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
        ///     Search for users using specific conditions.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SearchResult<IList<User>>> SearchUsersAsync(SearchUserViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Search for an user with specific conditions.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<User> SearchUserAsync(SearchUserViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Upload user profile image.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="photo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<User> UploadUserProfileImageAsync(int id, SKBitmap photo,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Change password asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<User> ChangePasswordAsync(int id, ChangePasswordViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Change user status using specific conditions.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<User> ChangeUserStatus(int id, ChangeUserStatusViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

      
        /// <summary>
        ///     Add | edit user signature asynchronously.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<User> AddUserSignatureAsync(AddUserSignatureViewModel model,
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