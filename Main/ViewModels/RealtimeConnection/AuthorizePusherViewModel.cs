using System.ComponentModel.DataAnnotations;

namespace Main.ViewModels.RealtimeConnection
{
    public class AuthorizePusherViewModel
    {
        #region Properties

        /// <summary>
        /// Name of pusher channel.
        /// </summary>
        [Required]
        public string ChannelName { get; set; }

        /// <summary>
        /// Id of socket connection.
        /// </summary>
        [Required]
        public string SocketId { get; set; }

        #endregion
    }
}