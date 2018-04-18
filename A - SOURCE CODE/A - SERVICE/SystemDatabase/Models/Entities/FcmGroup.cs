using System.ComponentModel.DataAnnotations;

namespace SystemDatabase.Models.Entities
{
    public class FcmGroup
    {
        #region Properties

        /// <summary>
        /// Name of group
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Key to send message to fcm server.
        /// </summary>
        public string MessagingKey { get; set; }

        /// <summary>
        /// Time when the group was created.
        /// </summary>
        public double CreatedTime { get; set; }

        #endregion
    }
}