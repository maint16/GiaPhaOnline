using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shared.ViewModels.SignalrConnections
{
    public class BroadcastSignalrNotificationViewModel
    {
        #region Properties

        /// <summary>
        /// Id of clients.
        /// </summary>
        public HashSet<string> Clients { get; set; }

        /// <summary>
        /// Name of method which should be raised on client side.
        /// </summary>
        [Required]
        public string MethodName { get; set; }

        /// <summary>
        /// Data to be sent to clients.
        /// </summary>
        public IDictionary Data { get; set; }

        #endregion
    }
}