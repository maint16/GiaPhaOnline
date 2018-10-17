using System;
using System.ComponentModel.DataAnnotations;

namespace AppShared.ViewModels.NotificationMessage
{
    public class GetNotificationMessageViewModel
    {
        #region Properties

        [Required]
        public Guid Id { get; set; }

        #endregion
    }
}