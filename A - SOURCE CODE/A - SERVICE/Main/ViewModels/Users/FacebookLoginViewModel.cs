using System.ComponentModel.DataAnnotations;

namespace Main.ViewModels.Users
{
    public class FacebookLoginViewModel
    {
        #region Properties

        /// <summary>
        /// Facebook code which has been returned by facebook api endpoint.
        /// </summary>
        [Required]
        public string Code { get; set; }

        #endregion
    }
}