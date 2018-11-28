using System.Collections.Generic;
using Newtonsoft.Json;

namespace MainShared.ViewModels.RealTime
{
    public class FcmPushMessageResponseViewModel
    {
        [JsonProperty("multicast_id")]
        public string MulticastId { get; set; }

        [JsonProperty("success")]
        public int Recipients { get; set; }

        [JsonProperty("failure")]
        public int FailedRecipients { get; set; }

        [JsonProperty("canonical_ids")]
        public int CanonicalIds { get; set; }

        [JsonProperty("results")]
        public List<FcmMessageResultViewModel> Results { get; set; }
    }
}