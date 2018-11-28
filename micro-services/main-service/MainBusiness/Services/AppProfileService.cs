using AppBusiness.Interfaces;
using AppDb.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using ServiceShared.Models;
using ServiceShared.Services;

namespace AppBusiness.Services
{
    public class AppProfileService : BaseProfileService, IAppProfileService
    {
        #region Constructor

        public AppProfileService(IOptions<AppJwtModel> appJwt, IHttpContextAccessor httpContextAccessor) : base(appJwt,
            httpContextAccessor)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public virtual User GetProfile()
        {
            return GetProfile<User>();
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="user"></param>
        public virtual void SetProfile(User user)
        {
            SetProfile<User>(user);
        }

        #endregion
    }
}