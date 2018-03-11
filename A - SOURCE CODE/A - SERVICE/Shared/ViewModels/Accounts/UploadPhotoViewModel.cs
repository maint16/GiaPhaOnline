using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.ViewModels.Accounts
{
    public class UploadPhotoViewModel
    {
        #region Properties

        /// <summary>
        /// Photo of Account. Should be formatted as (512x512)
        /// </summary>
        public string Image { get; set; }

        #endregion
    }
}
