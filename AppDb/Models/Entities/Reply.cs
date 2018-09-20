﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AppModel.Enumerations;
using Newtonsoft.Json;

namespace AppDb.Models.Entities
{
    public class Reply
    {
        #region Properties

        /// <summary>
        ///     Id of comment (Auto incremented)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Who wrote the comment.
        /// </summary>
        public int OwnerId { get; set; }

        /// <summary>
        ///     Which post this comment belongs to.
        /// </summary>
        public int TopicId { get; set; }

        /// <summary>
        ///     Category that reply belongs to.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        ///     Category group that reply belongs to.
        /// </summary>
        public int CategoryGroupId { get; set; }

        /// <summary>
        ///     Comment content.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Status of comment.
        /// </summary>
        public ItemStatus Status { get; set; }

        /// <summary>
        ///     When was the comment created.
        /// </summary>
        public double CreatedTime { get; set; }

        /// <summary>
        ///     When the comment was lastly modified.
        /// </summary>
        public double? LastModifiedTime { get; set; }

        #endregion

        #region Relationships

        /// <summary>
        ///     One comment can only be initiated by one account.
        /// </summary>
        public User Owner { get; set; }

        /// <summary>
        ///     One reply can only belong to one topic.
        /// </summary>
        public Topic Topic { get; set; }

        #endregion
    }
}