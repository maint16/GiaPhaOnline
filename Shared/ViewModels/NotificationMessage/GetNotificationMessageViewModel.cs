using System;
using System.ComponentModel.DataAnnotations;

namespace Shared.ViewModels.NotificationMessage
{
    public class GetNotificationMessageViewModel
    {
        #region Properties

        [Required]
        public Guid Id { get; set; }

        #endregion
    }
}