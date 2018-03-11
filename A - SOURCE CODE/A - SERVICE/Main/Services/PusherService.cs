using System.Threading.Tasks;
using Main.Interfaces.Services;
using Main.Models;
using Microsoft.Extensions.Options;
using PusherServer;

namespace Main.Services
{
    public class PusherService : IPusherService
    {
        #region Properties

        /// <summary>
        /// Instance of pusher service.
        /// </summary>
        private readonly IPusher _pusher;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize pusher service with injectors.
        /// </summary>
        /// <param name="appCluster"></param>
        /// <param name="appId"></param>
        /// <param name="appKey"></param>
        /// <param name="appSecret"></param>
        public PusherService(string appCluster, string appId, string appKey, string appSecret)
        {
            var options = new PusherOptions();
            options.Cluster = appCluster;

            _pusher = new Pusher(appId, appKey, appSecret, options);
        }

        /// <summary>
        /// Initialize pusher with injections.
        /// </summary>
        /// <param name="pusherSettingOptions"></param>
        public PusherService(IOptions<PusherSetting> pusherSettingOptions)
        {
            var pusherSetting = pusherSettingOptions.Value;
            var options = new PusherOptions();
            options.Cluster = pusherSetting.AppCluster;

            _pusher = new Pusher(pusherSetting.AppId, pusherSetting.AppKey, pusherSetting.AppSecret, options);
        }

        #endregion

        #region Methods

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Task<ITriggerResult> SendAsync(string socketId, string channelName, string eventName, object data)
        {
            // Initialize trigger option.
            TriggerOptions options = null;
            if (!string.IsNullOrWhiteSpace(socketId))
            {
                options = new TriggerOptions();
                options.SocketId = socketId;
            }

            return _pusher.TriggerAsync(channelName, eventName, data, options);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Task<ITriggerResult> SendAsync(string socketId, string[] channelNames, string eventName, object data)
        {
            // Initialize trigger option.
            TriggerOptions options = null;
            if (!string.IsNullOrWhiteSpace(socketId))
            {
                options = new TriggerOptions();
                options.SocketId = socketId;
            }

            return _pusher.TriggerAsync(channelNames, eventName, data, options);
        }

        /// <summary>
        /// Authenticate pusher client to a private channel.
        /// </summary>
        /// <param name="channelName"></param>
        /// <param name="socketId"></param>
        /// <param name="presenceChannelData"></param>
        /// <returns></returns>
        public IAuthenticationData Authenticate(string channelName, string socketId, PresenceChannelData presenceChannelData)
        {
            return _pusher.Authenticate(channelName, socketId, presenceChannelData);
        }
        
        #endregion
    }
}