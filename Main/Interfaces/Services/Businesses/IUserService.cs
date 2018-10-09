using System.Threading.Tasks;
using AppDb.Models.Entities;
using Main.ViewModels.Users;

namespace Main.Interfaces.Services.Businesses
{
    public interface IUserService
    {
        #region Methods

        /// <summary>
        /// Find user login information in system.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<User> LoginAsync(LoginViewModel model);

        /// <summary>
        /// Exchange google identity with user information.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<User> GoogleLoginAsync(GoogleLoginViewModel model);

        /// <summary>
        /// Exchange facebook identity with user information.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<User> FacebookLoginAsync(FacebookLoginViewModel model);

        /// <summary>
        /// Register user information basically.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<User> BasicRegisterAsync(RegisterAccountViewModel model);
        
        #endregion
    }
}