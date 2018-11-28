using System.ComponentModel.DataAnnotations.Schema;

namespace AppDb.Models.Entities
{
    public class Device
    {
        #region Properties

        /// <summary>
        /// Device id (token) which is for sending push notification to.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Device owner.
        /// </summary>
        public int? OwnerId { get; set; }

        /// <summary>
        /// Time when device was added to server.
        /// </summary>
        public double CreatedTime { get; set; }

        #endregion

        #region Relationships

        /// <summary>
        /// One owner can have multiple devices.
        /// </summary>
        [ForeignKey(nameof(OwnerId))]
        public virtual Account Owner { get; set; }

        #endregion
    }
}