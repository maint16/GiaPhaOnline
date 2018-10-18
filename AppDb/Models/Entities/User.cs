using System.Collections.Generic;
using ClientShared.Enumerations;
using Newtonsoft.Json;

namespace AppDb.Models.Entities
{
    public class User
    {
        #region Properties

        /// <summary>
        ///     Id of account (Auto incremented)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Email which is used for account registration.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     Nickname of account owner.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        ///     Password of account.
        /// </summary>
        [JsonIgnore]
        public string Password { get; set; }

        /// <summary>
        ///     Type of account
        /// </summary>
        public UserKind Type { get; set; }

        /// <summary>
        ///     Account status in the system.
        /// </summary>
        public UserStatus Status { get; set; }

        /// <summary>
        ///     Role of account
        /// </summary>
        public UserRole Role { get; set; }

        /// <summary>
        ///     Relative url (http url) of user photo.
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        ///     Signature of user.
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        ///     When was the account created.
        /// </summary>
        public double JoinedTime { get; set; }

        /// <summary>
        ///     When the account was lastly modified.
        /// </summary>
        public double? LastModifiedTime { get; set; }

        #endregion

        #region Relationships

        /// <summary>
        ///     Activation token which belongs to user.
        /// </summary>
        [JsonIgnore]
        public virtual ActivationToken ActivationToken { get; set; }

        /// <summary>
        ///     Token for user to access the system.
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

        /// <summary>
        ///     Token of devices that are used for sending notification.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<UserDeviceToken> DeviceTokens { get; set; }

        #endregion

        #region Constructor

        public User()
        {
        }

        public User(int id, string email, string nickname, string password, UserKind type, UserStatus status,
            UserRole role, string photo, string signature, double joinedTime, double? lastModifiedTime)
        {
            Id = id;
            Email = email;
            Nickname = nickname;
            Password = password;
            Type = type;
            Status = status;
            Role = role;
            Photo = photo;
            Signature = signature;
            JoinedTime = joinedTime;
            LastModifiedTime = lastModifiedTime;
        }

        #endregion
    }
}