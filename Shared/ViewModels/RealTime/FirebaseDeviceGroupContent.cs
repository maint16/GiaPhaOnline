﻿using Newtonsoft.Json;

namespace Shared.ViewModels.RealTime
{
    public class FirebaseDeviceGroupContent
    {
        #region Properties

        [JsonProperty("error")]
        public string ErrorMessage { get; set; }

        [JsonProperty("notification_key")]
        public string NotificationKey { get; set; }

        #endregion
    }
}