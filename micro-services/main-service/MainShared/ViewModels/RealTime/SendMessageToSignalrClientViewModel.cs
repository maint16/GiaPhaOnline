using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppShared.ViewModels.RealTime
{
    public class SendMessageToSignalrClientViewModel
    {
        #region Properties

        [Required]
        public string[] ClientIds { get; set; }

        [Required]
        public string EventName { get; set; }

        public Dictionary<string, object> Message { get; set; }

        #endregion
    }
}