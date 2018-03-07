using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Main.Interfaces.Services;
using Main.Models.PushNotification;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Main.Services
{
    public class FcmService : IPushNotificationService
    {

        #region Properties

        /// <summary>
        /// Fcm setting information.
        /// </summary>
        private readonly FcmSetting _fcmSetting;
        
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
            _fcmSetting = fcmSettingOptions.Value;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue($"key={_fcmSetting.ServerKey}");
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
        public async Task<HttpResponseMessage> SendNotification(FcmMessage fcmMessage)
        {
            // Set base url to make request to.
            _httpClient.BaseAddress = new Uri(UrlSendFcmNotification);

            // Initialize http content to send to FCM Service.
            var httpContent = new StringContent(JsonConvert.SerializeObject(fcmMessage));
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            // Make a request to notification server.
            return await _httpClient.PostAsync("", httpContent);
        }
        
        #endregion
    }
}