using System.Collections.Generic;
using SystemConstant.Enumerations;
using Newtonsoft.Json;

namespace SystemDatabase.Models.Entities
{
    public class Account
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
        public string Password { get; set; }

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
        public string PhotoRelativeUrl { get; set; }

        /// <summary>
        ///     Physical path of photo on the server.
        ///     This parameter should be ignored when data is sent back to client.
        /// </summary>
        [JsonIgnore]
        public string PhotoAbsoluteUrl { get; set; }

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
        ///     Token which belongs to user.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<Token> Tokens { get; set; }

        /// <summary>
        ///     List of categories created by this account.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<Category> InitializedCategories { get; set; }

        /// <summary>
        ///     List of categories user is following.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<FollowCategory> FollowCategories { get; set; }

        /// <summary>
        ///     List of posts user has created.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<Post> Posts { get; set; }

        /// <summary>
        ///     List of followed posts.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<FollowPost> FollowPosts { get; set; }

        /// <summary>
        ///     List of post report this account has.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<PostReport> OwnedPostReports { get; set; }

        /// <summary>
        ///     List of post reports user has created.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<PostReport> ReportedPosts { get; set; }

        /// <summary>
        ///     List of comments user has created.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<Comment> Comments { get; set; }

        /// <summary>
        ///     List of comment reports this account has reported.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<CommentReport> ReportedComments { get; set; }

        /// <summary>
        ///     List of comment reports this account owns.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<CommentReport> OwnedCommentReports { get; set; }

        /// <summary>
        ///     Comment notifications which should be received by this account.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<CommentNotification> ReceivedCommentNotifications { get; set; }

        /// <summary>
        ///     List of comment notification which have been broadcasted by the current user.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<CommentNotification> BroadcastedCommentNotifications { get; set; }

        /// <summary>
        ///     List of notifications which should be received by this account.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<PostNotification> ReceivedPostNotifications { get; set; }

        /// <summary>
        ///     List of post notifications broadcasted by this account.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<PostNotification> BroadcastedPostNotifications { get; set; }

        /// <summary>
        ///     List of signalr connections which have been established by this account.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<SignalrConnection> SignalrConnections { get; set; }

        #endregion
    }
}