using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Main.Interfaces.Services;
using Main.Models.PushNotification;
using Main.Models.PushNotification.Notification;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Main.Services
{
    public class FcmService : IPushService
    {

        #region Properties

        /// <summary>
        /// Fcm setting information.
        /// </summary>
        private readonly FcmSetting _fcmSetting;

        /// <summary>
        /// Snake case serializer setting.
        /// </summary>
        private readonly JsonSerializerSettings _snakeCaseSerializerSettings;

        /// <summary>
        /// Url to send FCM notification message.
        /// </summary>
        private const string UrlSendFcmNotification = "https://fcm.googleapis.com/fcm/send";

        /// <summary>
        /// Url which is for adding device to FCM.
        /// </summary>
        private const string UrlAddDeviceIntoGroup = "https://iid.googleapis.com/iid/v1/{device_id}/rel/topics/{topic_name}";

        /// <summary>
        /// Instance of http to make http request.
        /// </summary>
        private readonly HttpClient _httpClient;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize service with options.
        /// </summary>
        /// <param name="fcmSettingOptions"></param>
        public FcmService(IOptions<FcmSetting> fcmSettingOptions)
        {
            // Initialize snake case naming convention.
            _snakeCaseSerializerSettings = new JsonSerializerSettings();
            var contractResolver = new DefaultContractResolver();
            contractResolver.NamingStrategy = new SnakeCaseNamingStrategy();

            // Initialize contract resolver.
            _snakeCaseSerializerSettings.ContractResolver = contractResolver;

            _fcmSetting = fcmSettingOptions.Value;


            _httpClient = new HttpClient();
            //_httpClient.DefaultRequestHeaders.Add("Authorization", $"key={_fcmSetting.ServerKey}");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={_fcmSetting.ServerKey}");
            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("", $"key={_fcmSetting.ServerKey}");
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Add device to a specific group.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="group"></param>
        public async Task<HttpResponseMessage> AddDeviceToGroupAsync(string deviceId, string group)
        {
            var url = UrlAddDeviceIntoGroup;
            url = url.Replace("{device_id}", deviceId);
            url = url.Replace("{topic_name}", group);
            
            // Initialize http content.
            var httpContent = new StringContent("");

            // Call api to add device to group.
            _httpClient.BaseAddress = new Uri(url);
            return await _httpClient.PostAsync("", httpContent);
        }

        /// <summary>
        /// Send notification to a specific device.
        /// </summary>
        /// <param name="fcmMessage"></param>
        /// <param name="cancellationToken"></param>
        public async Task<HttpResponseMessage> SendNotification(FcmMessage fcmMessage, CancellationToken cancellationToken)
        {
            // Initialize http content.
            var szHttpContent = JsonConvert.SerializeObject(fcmMessage, _snakeCaseSerializerSettings);
            var httpContent = new StringContent(szHttpContent, Encoding.UTF8, "application/json");
            
            // Make a request to notification server.
            return await _httpClient.PostAsync(new Uri(UrlSendFcmNotification), httpContent, cancellationToken);
        }

        /// <summary>
        /// Send notification to 
        /// </summary>
        /// <param name="recipientIds"></param>
        /// <param name="notification"></param>
        /// <param name="collapseKey"></param>
        /// <param name="data"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> SendNotification(List<string> recipientIds, FcmBaseNotification notification, string collapseKey, IDictionary data, CancellationToken cancellationToken)
        {
            var fcmMessage = new FcmMessage();
            fcmMessage.RegistrationIds = recipientIds;
            fcmMessage.Notification = notification;
            fcmMessage.CollapseKey = collapseKey;
            fcmMessage.Data = data;

            return await SendNotification(fcmMessage, cancellationToken);
        }

        #endregion
    }
}