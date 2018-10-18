﻿using System.ComponentModel.DataAnnotations;

namespace AppShared.ViewModels.Topic
{
    public class AddTopicViewModel
    {
        #region Properties

        /// <summary>
        ///     Category that category belongs to.
        /// </summary>
        [Required]
        public int CategoryId { get; set; }

        /// <summary>
        ///     Title of topic
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        ///     Body of topic
        /// </summary>
        [Required]
        public string Body { get; set; }

        #endregion
    }
}