﻿using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppBusiness.Models.NotificationMessages;
using AppDb.Models.Entities;
using Shared.Models;
using Shared.ViewModels.NotificationMessage;

namespace AppBusiness.Interfaces.Domains
{
    public interface INotificationMessageDomain
    {
        #region Methods

        /// <summary>
        /// Add notification message to system.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<NotificationMessage> AddNotificationMessageAsync<T>(AddNotificationMessageModel<T> model, CancellationToken cancellationToken = default (CancellationToken));

        /// <summary>
        /// Get notification message using id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<NotificationMessage> GetNotificationMessageUsingId(string id,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Search for notification messages using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SearchResult<IList<NotificationMessage>>> SearchNotificationMessagesAsync(SearchNotificationMessageViewModel condition, CancellationToken cancellationToken = default (CancellationToken));

        #endregion
    }
}