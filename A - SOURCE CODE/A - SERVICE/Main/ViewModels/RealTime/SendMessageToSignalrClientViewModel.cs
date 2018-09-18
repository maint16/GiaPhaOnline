using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Main.Models.RealTime;

namespace Main.ViewModels.RealTime
{
    public class SendMessageToSignalrClientViewModel
    {
        #region Properties

        [Required]
        public string[] ClientIds { get; set; }

        [Required]
        public string EventName { get; set; }

        public RealTimeMessage<Dictionary<string, object>> Message { get; set; }

        #endregion
    }
}