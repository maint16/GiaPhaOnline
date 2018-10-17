using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AppShared.ViewModels.Users
{
    public class UploadPhotoViewModel
    {
        #region Properties

        /// <summary>
        ///     Id of user whose avatar will be updated.
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        ///     User photo
        /// </summary>
        [Required]
        public IFormFile Photo { get; set; }

        #endregion
    }
}