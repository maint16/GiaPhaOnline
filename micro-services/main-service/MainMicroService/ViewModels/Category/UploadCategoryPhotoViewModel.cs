using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Main.ViewModels.Category
{
    public class UploadCategoryPhotoViewModel
    {
        #region Properties

        public int CategoryId { get; set; }

        [Required]
        public IFormFile Photo { get; set; }

        #endregion
    }
}