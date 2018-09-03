using System.ComponentModel.DataAnnotations;

namespace Main.ViewModels.Users
{
    public class GoogleLoginViewModel
    {
        #region Properties

        /// <summary>
        /// Google authentication code.
        /// </summary>
        [Required]
        public string Code { get; set; }

        #endregion
    }
}