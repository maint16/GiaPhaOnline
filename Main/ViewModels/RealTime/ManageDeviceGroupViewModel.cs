using Newtonsoft.Json;

namespace Main.ViewModels.RealTime
{
    public class ManageDeviceGroupViewModel
    {
        public string Operation { get; set; }

        [JsonProperty("notification_key_name")]
        public string NotificationKeyName { get; set; }

        [JsonProperty("notification_key")]
        public string NotificationKey { get; set; }

        [JsonProperty("registration_ids")]
        public string[] RegistrationIds { get; set; }
    }
}