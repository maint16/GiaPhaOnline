using Newtonsoft.Json;

namespace AppShared.ViewModels.RealTime
{
    public class FcmMessageResultViewModel
    {
        /// <summary>
        ///     Id of message that has been sent to client successfully.
        /// </summary>
        [JsonProperty("message_id")]
        public string MessageId { get; set; }

        /// <summary>
        ///     Reason for the failed message.
        ///     For more information: https://firebase.google.com/docs/cloud-messaging/http-server-ref#table9
        /// </summary>
        [JsonProperty("error")]
        public string Error { get; set; }
    }
}