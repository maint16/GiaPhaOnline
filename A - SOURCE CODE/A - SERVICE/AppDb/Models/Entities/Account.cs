using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AppModel.Enumerations;
using Newtonsoft.Json;

namespace AppDb.Models.Entities
{
    public class Account
    {
        #region Properties

        /// <summary>
        ///     Id of account (Auto incremented)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        ///     Email which is used for account registration.
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        ///     Nickname of account owner.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        ///     Password of account.
        /// </summary>
        [JsonIgnore]
        [Required]
        public string Password { get; set; }

        /// <summary>
        ///     Type of account
        /// </summary>
        public AccountType Type { get; set; }

        /// <summary>
        ///     Account status in the system.
        /// </summary>
        public AccountStatus Status { get; set; }

        /// <summary>
        ///     Role of account
        /// </summary>
        public AccountRole Role { get; set; }

        /// <summary>
        ///     Relative url (http url) of user photo.
        /// </summary>
        [JsonProperty("Photo")]
        public string Photo { get; set; }

        /// <summary>
        ///     When was the account created.
        /// </summary>
        [Required]
        public double JoinedTime { get; set; }

        /// <summary>
        ///     When the account was lastly modified.
        /// </summary>
        public double? LastModifiedTime { get; set; }

        #endregion

        #region Relationships

        /// <summary>
        ///     Token which belongs to user.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<AccessToken> AccessTokens { get; set; }

        /// <summary>
        ///     List of group categories created by this account.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<CategoryGroup> CategoryGroups { get; set; }

        /// <summary>
        ///     List of categories created by this account.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<Category> Categories { get; set; }

        /// <summary>
        ///     List of categories user is following.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<FollowCategory> FollowCategories { get; set; }

        /// <summary>
        ///     List of topics user has created.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<Topic> Topics { get; set; }

        /// <summary>
        ///     List of followed posts.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<FollowTopic> FollowTopics { get; set; }

        /// <summary>
        ///     List of topic report this account has.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<ReportTopic> OwnedTopicReports { get; set; }

        /// <summary>
        ///     List of topic reports user has created.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<ReportTopic> ReportedPosts { get; set; }

        /// <summary>
        ///     List of replies user has created.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<Reply> Replies { get; set; }

        /// <summary>
        ///     List of notifications user has created.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<NotificationMessage> NotificationMessages { get; set; }

        #endregion
    }
}