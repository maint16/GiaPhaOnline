using System;
using System.ComponentModel.DataAnnotations;

namespace MainShared.ViewModels.NotificationMessage
{
    public class GetNotificationMessageViewModel
    {
        #region Properties

        [Required]
        public Guid Id { get; set; }

        #endregion
    }
}