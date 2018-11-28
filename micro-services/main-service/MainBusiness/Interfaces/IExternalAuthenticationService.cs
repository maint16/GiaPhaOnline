using System.Threading.Tasks;
using MainModel.Models.ExternalAuthentication;

namespace MainBusiness.Interfaces
{
    public interface IExternalAuthenticationService
    {
        #region Methods

        /// <summary>
        ///     Get google token information from code.
        ///     To know how to get the code. Please refer : https://developers.google.com/identity/protocols/OAuth2WebServer
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<GoogleTokenInfo> GetGoogleTokenInfoAsync(string code);

        /// <summary>
        ///     Get facebook token information from code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<FacebookTokenInfo> GetFacebookTokenInfoAsync(string code);

        /// <summary>
        ///     Convert id token to basic profile.
        /// </summary>
        /// <param name="idToken"></param>
        /// <returns></returns>
        Task<GoogleProfile> GetGoogleBasicProfileAsync(string idToken);

        /// <summary>
        ///     Exchange access token for facebook basic profile.
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        Task<FacebookProfile> GetFacebookBasicProfileAsync(string accessToken);

        #endregion
    }
}