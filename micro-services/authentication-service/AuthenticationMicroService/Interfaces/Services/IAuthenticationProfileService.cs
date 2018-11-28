using AuthenticationDb.Models.Entities;
using ServiceShared.Interfaces.Services;

namespace AuthenticationMicroService.Interfaces.Services
{
    public interface IAuthenticationProfileService : IBaseProfileService
    {
        #region Properties

        /// <summary>
        ///     Get user profile from request.
        /// </summary>
        /// <returns></returns>
        User GetProfile();

        /// <summary>
        ///     Set user profile to request.
        /// </summary>
        /// <param name="user"></param>
        void SetProfile(User user);

        #endregion
    }
}