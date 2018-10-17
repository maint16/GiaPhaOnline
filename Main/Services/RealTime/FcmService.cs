using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AppDb.Interfaces;
using AppShared.ViewModels.RealTime;
using Main.Constants;
using Main.Constants.RealTime;
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
            _httpClient = httpClientFactory.CreateClient(HttpClientGroupConstant.FcmService);
            _httpClient.BaseAddress = new Uri(UrlFcmBaseUrl);

            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Properties

        ///// <summary>
        /////     Fcm setting information.
        ///// </summary>
        //private readonly FcmOption _fcmOption;

        /// <summary>
        ///     Instance to access database and its entities.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        ///     Snake case serializer setting.
        /// </summary>
        private readonly JsonSerializerSettings _snakeCaseSerializerSettings;

        /// <summary>
        ///     Base url of FCM service.
        /// </summary>
        private const string UrlFcmBaseUrl = "https://fcm.googleapis.com";

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

        /// <summary>
        ///     Url to get token information.
        /// </summary>
        private const string UrlGetCloudMessagingTokenInformation =
            "https://iid.googleapis.com/iid/info/{0}";

        /// <summary>
        ///     Url to find device group notification key.
        /// </summary>
        private const string UrlFindDeviceGroupNotificationKey = "fcm/notification?notification_key_name={0}";

        /// <summary>
        ///     Url to manage device group.
        /// </summary>
        private const string UrlDeviceGroupManagement = "fcm/notification";

        private readonly IHttpClientFactory _httpClientFactory;

        private readonly HttpClient _httpClient;

        #endregion

        #region Methods

        /// <summary>
        ///     Get device group notification key by using notification key name.
        /// </summary>
        /// <param name="notificationKeyName"></param>
        /// <returns></returns>
        public async Task<string> GetDeviceGroupNotificationKey(string notificationKeyName)
        {
            var subUrl = string.Format(UrlFindDeviceGroupNotificationKey, notificationKeyName);
            var httpResponseMessage = await _httpClient.GetAsync(new Uri(subUrl));
            var httpContent = httpResponseMessage.Content;

            if (httpContent == null)
                throw new Exception("No content responded from service.");

            var content = await httpContent.ReadAsAsync<FirebaseDeviceGroupContent>();
            if (httpResponseMessage.IsSuccessStatusCode)
                return content.NotificationKey;

            if (FcmErrorMessageConstant.NotificationKeyNotFound.Equals(content.ErrorMessage))
                return null;

            throw new Exception("Invalid server response");
        }

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
        /// <param name="deviceIds"></param>
        /// <param name="group"></param>
        /// <param name="cancellationToken"></param>
        public async Task<HttpResponseMessage> AddDevicesToGroupAsync(string[] deviceIds, string group,
            CancellationToken cancellationToken)
        {
            #region Use topic instead (deprecated)

            // Due to the fcm complexity of firebase cloud messaging group. 
            // Use topics instead.
            //return await AddDeviceToTopicAsync(deviceId, group, cancellationToken);

            #endregion

            // Get group notification key.
            var notificationKey = await GetDeviceGroupNotificationKey(group);

            // Create model to submit to fcm service.
            var model = new ManageDeviceGroupViewModel();
            model.NotificationKey = group;
            model.RegistrationIds = deviceIds;

            // No notification key is found.
            if (string.IsNullOrEmpty(notificationKey))
            {
                model.Operation = "create";
            }
            else
            {
                model.Operation = "add";
                model.NotificationKeyName = notificationKey;
            }

            // Initialize uri.
            var subUri = new Uri(UrlDeviceGroupManagement);
            return await _httpClient.PostAsJsonAsync(subUri, model, cancellationToken);
        }

        /// <summary>
        ///     Add devices to groups
        /// </summary>
        /// <param name="deviceIds"></param>
        /// <param name="groups"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IList<HttpResponseMessage>> AddDevicesToGroupsAsync(string[] deviceIds,
            IList<string> groups, CancellationToken cancellationToken)
        {
            // Initialize background tasks.
            var backgroundTasks = new List<Task>();

            // Intialize list of http response message.
            var httpResponseMessages = new List<HttpResponseMessage>();

            foreach (var group in groups)
            {
                var pAddDevicesToGroupTask = AddDevicesToGroupAsync(deviceIds, group, cancellationToken)
                    .ContinueWith(httpResponseMessageTask => httpResponseMessages.Add(httpResponseMessageTask.Result),
                        cancellationToken);
                backgroundTasks.Add(pAddDevicesToGroupTask);
            }

            await Task.WhenAll(backgroundTasks.ToArray());
            return httpResponseMessages;
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

#if !DEBUG // Make a request to notification server.
            var httpClient = _httpClientFactory.CreateClient(HttpClientGroupConstant.FcmService);
            return await httpClient.PostAsync(new Uri(UrlSendFcmNotification), httpContent, cancellationToken);
#else
            // Make a request to notification server.
            var httpClient = _httpClientFactory.CreateClient(HttpClientGroupConstant.FcmService);
            var httpResponseMessage =
                await httpClient.PostAsync(new Uri(UrlSendFcmNotification), httpContent, cancellationToken);
            return httpResponseMessage;
#endif
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
        ///     <inheritdoc />
        /// </summary>
        /// <param name="idToken"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<CloudMessagingTokenInfoViewModel> GetCloudMessagingTokenInformationAsync(string idToken,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var httpClient = _httpClientFactory.CreateClient(HttpClientGroupConstant.FcmService);
            var url = string.Format(UrlGetCloudMessagingTokenInformation, idToken);
            httpClient.BaseAddress = new Uri(url);
            var httpResponseMessage = await httpClient.GetAsync("", cancellationToken);
            if (!httpResponseMessage.IsSuccessStatusCode)
                return null;

            var httpContent = httpResponseMessage.Content;
            if (httpContent == null)
                return null;

            return await httpContent.ReadAsAsync<CloudMessagingTokenInfoViewModel>(cancellationToken);
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