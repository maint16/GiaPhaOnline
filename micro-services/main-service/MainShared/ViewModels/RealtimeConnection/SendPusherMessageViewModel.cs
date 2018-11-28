using System.ComponentModel.DataAnnotations;

namespace MainShared.ViewModels.RealtimeConnection
{
    public class SendPusherMessageViewModel
    {
        #region Properties

        /// <summary>
        ///     Id of socket.
        /// </summary>
        public string SocketId { get; set; }

        /// <summary>
        ///     Channel name.
        /// </summary>
        [Required]
        public string ChannelName { get; set; }

        /// <summary>
        ///     Event name.
        /// </summary>
        [Required]
        public string EventName { get; set; }

        /// <summary>
        ///     Extra information
        /// </summary>
        public object Information { get; set; }

        #endregion
    }
}