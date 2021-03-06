﻿using ClientShared.Enumerations;
using Newtonsoft.Json;

namespace AuthenticationDb.Models.Entities
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
        ///     Relative url (http url) of user photo.
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        ///     Account status in the system.
        /// </summary>
        public UserStatus Status { get; set; }

        /// <summary>
        ///     Type of account
        /// </summary>
        public UserKind Type { get; set; }

        /// <summary>
        ///     Role of account
        /// </summary>
        public UserRole Role { get; set; }

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

        #region Constructor

        public User()
        {
        }

        public User(int id, string email, string nickname, string password, string photo, UserStatus status,
            UserKind type, UserRole role, string signature, double joinedTime, double? lastModifiedTime)
        {
            Id = id;
            Email = email;
            Nickname = nickname;
            Password = password;
            Photo = photo;
            Status = status;
            Type = type;
            Role = role;
            Signature = signature;
            JoinedTime = joinedTime;
            LastModifiedTime = lastModifiedTime;
        }

        #endregion
    }
}