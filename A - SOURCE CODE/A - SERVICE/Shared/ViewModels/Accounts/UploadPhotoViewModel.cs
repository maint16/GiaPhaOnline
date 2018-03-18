using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Shared.ViewModels.Accounts
{
    public class UploadPhotoViewModel
    {
        #region Properties

        /// <summary>
        ///     Photo of Account. Should be formatted as (512x512)
        /// </summary>
        [Required]
        public IFormFile Image { get; set; }

        #endregion
    }
}