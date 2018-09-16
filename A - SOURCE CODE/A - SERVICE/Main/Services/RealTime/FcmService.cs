using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AppDb.Interfaces;
using Main.Constants;
using Main.Interfaces.Services;
using Main.Models.PushNotification;
using Main.Models.PushNotification.Notification;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Main.Services.RealTime
{
    public class FcmService : ICloudMessagingService
    {
        #region Constructor

        /// <summary>
        ///     Initialize service with options.
        /// </summary>
        /// <param name="httpClientFactory"></param>
        /// <param name="unitOfWork"></param>
        public FcmService(IHttpClientFactory httpClientFactory, IUnitOfWork unitOfWork)
        {
            // Initialize snake case naming convention.
            _snakeCaseSerializerSettings = new JsonSerializerSettings();
            var contractResolver = new DefaultContractResolver();
            contractResolver.NamingStrategy = new SnakeCaseNamingStrategy();

            // Initialize contract resolver.
            _snakeCaseSerializerSettings.ContractResolver = contractResolver;

            _httpClientFactory = httpClientFactory;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Fcm setting information.
        /// </summary>
        private readonly FcmOption _fcmOption;

        /// <summary>
        ///     Instance to access database and its entities.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        ///     Snake case serializer setting.
        /// </summary>
        private readonly JsonSerializerSettings _snakeCaseSerializerSettings;

        /// <summary>
        ///     Url to send FCM notification message.
        /// </summary>
        private const string UrlSendFcmNotification = "https://fcm.googleapis.com/fcm/send";

        /// <summary>
        ///     Url which is for adding devices into a specific topic.
        /// </summary>
        private const string UrlAddDevicesIntoTopic = "https://iid.googleapis.com/iid/v1:batchAdd";

        /// <summary>
        ///     Url which is for removing devices from a specific topic.
        /// </summary>
        private const string UrlDeleteDevicesFromTopic = "https://iid.googleapis.com/iid/v1:batchRemove";

        private readonly IHttpClientFactory _httpClientFactory;

        #endregion

        #region Methods

        /// <summary>
        ///     Add device to a specific group.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="topic"></param>
        /// <param name="cancellationToken"></param>
        public async Task<HttpResponseMessage> AddDeviceToTopicAsync(string deviceId, string topic,
            CancellationToken cancellationToken)
        {
            var httpResponseMessages = await AddDevicesToTopics(new List<string> {deviceId}, new List<string> {topic},
                cancellationToken);

            if (httpResponseMessages == null || httpResponseMessages.Count < 1)
                return null;

            return httpResponseMessages[0];
        }

        /// <summary>
        ///     Add devices to specific groups.
        /// </summary>
        /// <param name="deviceIds"></param>
        /// <param name="topics"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IList<HttpResponseMessage>> AddDevicesToTopics(IList<string> deviceIds, IList<string> topics,
            CancellationToken cancellationToken)
        {
            // List of tasks that must be finished.
            var addDeviceToGroupsTasks = new List<Task<HttpResponseMessage>>();

            // Go through every device
            foreach (var topic in topics)
            {
                var data = new Dictionary<string, object>();
                data.Add("to", $"/topics/{topic}");
                data.Add("registration_tokens", deviceIds);

                // Initialize http content.
                var httpContent = new StringContent(JsonConvert.SerializeObject(data));

                var httpClient = _httpClientFactory.CreateClient(HttpClientGroupConstant.FcmService);
                httpClient.BaseAddress = new Uri(UrlAddDevicesIntoTopic);
                var addDevicesToGroupTask = httpClient.PostAsync("", httpContent, cancellationToken);
                addDeviceToGroupsTasks.Add(addDevicesToGroupTask);
            }

            var httpResponseMessages = await Task.WhenAll(addDeviceToGroupsTasks);
            return httpResponseMessages;
        }

        /// <summary>
        ///     Add device to a specific group.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="group"></param>
        /// <param name="cancellationToken"></param>
        public async Task<HttpResponseMessage> AddDeviceToGroupAsync(string deviceId, string group,
            CancellationToken cancellationToken)
        {
            // Due to the fcm complexity of firebase cloud messaging group. 
            // Use topics instead.
            return await AddDeviceToTopicAsync(deviceId, group, cancellationToken);
        }

        /// <summary>
        ///     Add devices to groups
        /// </summary>
        /// <param name="deviceIds"></param>
        /// <param name="groups"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IList<HttpResponseMessage>> AddDeviceToGroupAsync(IList<string> deviceIds,
            IList<string> groups, CancellationToken cancellationToken)
        {
            return await AddDevicesToTopics(deviceIds, groups, cancellationToken);
        }

        /// <summary>
        ///     Send notification to a specific device.
        /// </summary>
        /// <param name="fcmMessage"></param>
        /// <param name="cancellationToken"></param>
        public async Task<HttpResponseMessage> SendAsync<T>(FcmMessage<T> fcmMessage,
            CancellationToken cancellationToken)
        {
            // Initialize http content.
            var szHttpContent = JsonConvert.SerializeObject(fcmMessage, _snakeCaseSerializerSettings);
            var httpContent = new StringContent(szHttpContent, Encoding.UTF8, "application/json");

            // Make a request to notification server.
            var httpClient = _httpClientFactory.CreateClient(HttpClientGroupConstant.FcmService);
            return await httpClient.PostAsync(new Uri(UrlSendFcmNotification), httpContent, cancellationToken);
        }

        /// <summary>
        ///     Send notification to
        /// </summary>
        /// <param name="recipientIds"></param>
        /// <param name="notification"></param>
        /// <param name="collapseKey"></param>
        /// <param name="data"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> SendAsync<T>(List<string> recipientIds, FcmBaseNotification notification,
            string collapseKey, T data, CancellationToken cancellationToken)
        {
            var fcmMessage = new FcmMessage<T>();
            fcmMessage.RegistrationIds = recipientIds;
            fcmMessage.Notification = notification;
            fcmMessage.CollapseKey = collapseKey;
            fcmMessage.Data = data;

            return await SendAsync(fcmMessage, cancellationToken);
        }

        /// <summary>
        ///     Delete devices from groups.
        /// </summary>
        /// <param name="deviceIds"></param>
        /// <param name="topics"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IList<HttpResponseMessage>> DeleteDevicesFromTopicsAsync(IList<string> deviceIds,
            IList<string> topics, CancellationToken cancellationToken)
        {
            // List of tasks that must be finished.
            var addDeviceToGroupsTasks = new List<Task<HttpResponseMessage>>();

            // Go through every device
            foreach (var topic in topics)
            {
                var data = new Dictionary<string, object>();
                data.Add("to", $"/topics/{topic}");
                data.Add("registration_tokens", deviceIds);

                // Initialize http content.
                var httpContent = new StringContent(JsonConvert.SerializeObject(data));

                var httpClient = _httpClientFactory.CreateClient(HttpClientGroupConstant.FcmService);
                httpClient.BaseAddress = new Uri(UrlDeleteDevicesFromTopic);
                var addDevicesToGroupTask = httpClient.PostAsync("", httpContent, cancellationToken);
                addDeviceToGroupsTasks.Add(addDevicesToGroupTask);
            }

            var httpResponseMessages = await Task.WhenAll(addDeviceToGroupsTasks);
            return httpResponseMessages;
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="group"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> DeleteDeviceFromGroupAsync(string deviceId, string group,
            CancellationToken cancellationToken)
        {
            var httpResponseMessages = await DeleteDevicesFromTopicsAsync(new List<string> {deviceId},
                new List<string> {group}, cancellationToken);

            if (httpResponseMessages == null || httpResponseMessages.Count < 1)
                return null;

            return httpResponseMessages[0];
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="deviceIds"></param>
        /// <param name="groups"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IList<HttpResponseMessage>> DeleteDevicesFromGroupsAsync(IList<string> deviceIds,
            IList<string> groups, CancellationToken cancellationToken)
        {
            return await DeleteDevicesFromTopicsAsync(deviceIds, groups, cancellationToken);
        }

        #endregion
    }
}