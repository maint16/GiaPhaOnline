using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Main.Models.RealTime;

namespace Main.ViewModels.RealTime
{
    public class SendMessageToSignalGroupViewModel
    {
        #region Properties

        [Required]
        public string[] Groups { get; set; }

        [Required]
        public string EventName { get; set; }

        public T<Dictionary<string, object>> Message { get; set; }

        #endregion
    }
}