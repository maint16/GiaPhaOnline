using AuthenticationDb.Models.Entities;
using AuthenticationMicroService.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using ServiceShared.Models;
using ServiceShared.Services;

namespace AuthenticationMicroService.Services
{
    public class AuthenticationProfileService : BaseProfileService, IAuthenticationProfileService
    {
        #region Constructor

        public AuthenticationProfileService(IOptions<AppJwtModel> appJwt, IHttpContextAccessor httpContextAccessor) :
            base(appJwt, httpContextAccessor)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public User GetProfile()
        {
            return GetProfile<User>();
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="user"></param>
        public void SetProfile(User user)
        {
            SetProfile<User>(user);
        }

        #endregion
    }
}