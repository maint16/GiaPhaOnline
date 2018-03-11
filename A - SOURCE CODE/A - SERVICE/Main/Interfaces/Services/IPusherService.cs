using System.Collections.Generic;
using System.Threading.Tasks;
using PusherServer;

namespace Main.Interfaces.Services
{
    public interface IPusherService
    {
        #region Methods

        /// <summary>
        /// Send push notification to clients. 
        /// </summary>
        /// <param name="socketId"></param>
        /// <param name="channelName"></param>
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<ITriggerResult> SendAsync(string socketId, string channelName, string eventName, object data = null);

        /// <summary>
        /// Send push notification to multiple channels.
        /// </summary>
        /// <param name="socketId"></param>
        /// <param name="channelNames"></param>
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<ITriggerResult> SendAsync(string socketId, string[] channelNames, string eventName, object data = null);

        /// <summary>
        /// Authenticate pusher client to a private channel.
        /// </summary>
        /// <param name="channelName"></param>
        /// <param name="socketId"></param>
        /// <param name="presenceChannelData"></param>
        /// <returns></returns>
        IAuthenticationData Authenticate(string channelName, string socketId, PresenceChannelData presenceChannelData = null);

        #endregion
    }
}