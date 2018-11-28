using AppDb.Models.Entities;
using ServiceShared.Interfaces.Services;

namespace AppBusiness.Interfaces
{
    public interface IAppProfileService : IBaseProfileService
    {
        #region Methods

        /// <summary>
        ///     Get profile from request.
        /// </summary>
        /// <returns></returns>
        User GetProfile();

        /// <summary>
        ///     Set profile to request.
        /// </summary>
        /// <param name="user"></param>
        void SetProfile(User user);

        #endregion
    }
}