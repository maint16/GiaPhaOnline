﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace AppDb.Models.Entities
{
    public class SignalrConnection
    {
        #region Relationship

        /// <summary>
        ///     Owner of connection.
        /// </summary>
        [JsonIgnore]
        [ForeignKey(nameof(OwnerId))]
        public Account Owner { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Connection index (auto generated by SignalR)
        /// </summary>
        [Key]
        public string Id { get; set; }
        
        /// <summary>
        ///     Id of account which created the connection.
        /// </summary>
        public int OwnerId { get; set; }

        /// <summary>
        ///     When the connection was created.
        /// </summary>
        public double CreatedTime { get; set; }

        #endregion
    }
}