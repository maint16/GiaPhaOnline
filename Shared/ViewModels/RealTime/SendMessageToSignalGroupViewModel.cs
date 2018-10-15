using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shared.ViewModels.RealTime
{
    public class SendMessageToSignalGroupViewModel
    {
        #region Properties

        [Required]
        public string[] Groups { get; set; }

        [Required]
        public string EventName { get; set; }

        public Dictionary<string, object> Message { get; set; }

        #endregion
    }
}