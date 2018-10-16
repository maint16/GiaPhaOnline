using System.ComponentModel.DataAnnotations;

namespace Shared.ViewModels.NotificationMessage
{
    public class GetNotificationMessageViewModel
    {
        #region Properties

        [Required]
        [RegularExpression(@"^[{(]?[0-9A-F]{8}[-]?(?:[0-9A-F]{4}[-]?){3}[0-9A-F]{12}[)}]?$")]
        public string Id { get; set; }

        #endregion
    }
}