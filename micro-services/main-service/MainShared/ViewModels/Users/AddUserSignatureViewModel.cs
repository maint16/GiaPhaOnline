using System.ComponentModel.DataAnnotations;

namespace MainShared.ViewModels.Users
{
    public class AddUserSignatureViewModel
    {
        #region Properties

        public int? UserId { get; set; }

        /// <summary>
        ///     User signature.
        /// </summary>
        [Required]
        public string Signature { get; set; }

        #endregion
    }
}