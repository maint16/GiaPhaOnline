using System.Collections;
using System.Collections.Generic;
using Main.Models.PushNotification.Notification;
using Newtonsoft.Json;

namespace Main.Models.PushNotification
{
    public class FcmMessage
    {
        #region Properties

        /// <summary>
        /// This parameter specifies the recipient of a message.
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// This parameter specifies the recipient of a multicast message, a message sent to more than one registration token.
        /// The value should be an array of registration tokens to which to send the multicast message. 
        /// The array must contain at least 1 and at most 1000 registration tokens. To send a message to a single device, use the to parameter.
        /// </summary>
        public List<string> RegistrationIds { get; set; }

        /// <summary>
        /// This parameter specifies a logical expression of conditions that determine the message target.
        /// Supported condition: Topic, formatted as "'yourTopic' in topics". This value is case-insensitive.
        /// Supported operators: &amp;&amp;, ||. Maximum two operators per topic message supported.
        /// </summary>
        public string Condition { get; set; }

        /// <summary>
        /// This parameter identifies a group of messages (e.g., with collapse_key: "Updates Available") that can be collapsed, so that only the last message gets sent when delivery can be resumed. 
        /// This is intended to avoid sending too many of the same messages when the device comes back online or becomes active.
        /// </summary>
        public string CollapseKey { get; set; }
        
        /// <summary>
        /// Sets the priority of the message. Valid values are "normal" and "high." On iOS, these correspond to APNs priorities 5 and 10.
        /// By default, notification messages are sent with high priority, and data messages are sent with normal priority. Normal priority optimizes the client app's battery consumption and should be used unless immediate delivery is required. For messages with normal priority, the app may receive the message with unspecified delay.
        /// When a message is sent with high priority, it is sent immediately, and the app can display a notification.
        /// </summary>
        public string Priority { get; set; }

        /// <summary>
        /// Sets the priority of the message. Valid values are "normal" and "high." On iOS, these correspond to APNs priorities 5 and 10.
        /// By default, notification messages are sent with high priority, and data messages are sent with normal priority. 
        /// Normal priority optimizes the client app's battery consumption and should be used unless immediate delivery is required. For messages with normal priority, the app may receive the message with unspecified delay.
        /// </summary>
        public bool ContentAvailable { get; set; }

        /// <summary>
        /// This parameter specifies how long (in seconds) the message should be kept in FCM storage if the device is offline. The maximum time to live supported is 4 weeks, and the default value is 4 weeks
        /// For more information: https://firebase.google.com/docs/cloud-messaging/concept-options#ttl
        /// </summary>
        public int? TimeToLive { get; set; }

        /// <summary>
        /// This parameter specifies the package name of the application where the registration tokens must match in order to receive the message
        /// </summary>
        public string RestrictedPackageName { get; set; }

        /// <summary>
        /// This parameter, when set to true, allows developers to test a request without actually sending a message.
        /// </summary>
        public bool DryRun { get; set; } = false;

        /// <summary>
        /// This parameter specifies the custom key-value pairs of the message's payload.
        /// For example, with data:{"score":"3x1"}:
        /// </summary>
        public IDictionary Data { get; set; }

        /// <summary>
        /// Fcm notification
        /// This parameter specifies the predefined, user-visible key-value pairs of the notification payload. See Notification payload support for detail. 
        /// For more information about notification message and data message options, see Message types.
        /// If a notification payload is provided, or the content_available option is set to true for a message to an iOS device, the message is sent through APNs, otherwise it is sent through the FCM connection server
        /// </summary>
        public FcmBaseNotification Notification { get; set; }

        #endregion
    }
}