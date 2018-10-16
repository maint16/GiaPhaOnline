using System.ComponentModel.DataAnnotations;

namespace AuthenticationShared.ViewModels.User
{
    public class FacebookLoginViewModel
    {
        #region Properties

        /// <summary>
        ///     Facebook code which has been returned by facebook api endpoint.
        /// </summary>
        [Required]
        public string AccessToken { get; set; }

        #endregion
    }
}