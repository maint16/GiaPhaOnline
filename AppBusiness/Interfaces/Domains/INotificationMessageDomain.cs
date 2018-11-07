using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppBusiness.Models.NotificationMessages;
using AppDb.Models.Entities;
using AppModel.Enumerations;
using AppShared.ViewModels.NotificationMessage;
using ClientShared.Models;

namespace AppBusiness.Interfaces.Domains
{
    public interface INotificationMessageDomain
    {
        #region Methods

        /// <summary>
        ///     Add notification message to system.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="bIsExpressionSupressed"></param>
        /// <returns></returns>
        Task<NotificationMessage> AddNotificationMessageAsync<T>(
            AddNotificationMessageModel<T> model,
            CancellationToken cancellationToken = default(CancellationToken),
            bool bIsExpressionSupressed = default(bool));

        /// <summary>
        ///     Add notification message list of user.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="bIsExpressionSupressed"></param>
        /// <returns></returns>
        Task AddNotificationMessageToListUser<T>(
            AddListUserNotificationMessageModel<T> model,
            CancellationToken cancellationToken = default(CancellationToken),
            bool bIsExpressionSupressed = default(bool));

        /// <summary>
        /// Add notification message to a group of user.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="userGroup"></param>
        /// <param name="model"></param>
        /// <param name="bIsExceptionSuppressed"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task AddNotificationMessageToUserGroup<T>(UserGroup userGroup, AddUserGroupNotificationMessageModel<T> model, bool bIsExceptionSuppressed = false, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Get notification message using id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<NotificationMessage> GetNotificationMessageUsingId(Guid id,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Mark notification as seen message using id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<NotificationMessage> MarkNotificationMessageAsSeen(Guid id,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Search for notification messages using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SearchResult<IList<NotificationMessage>>> SearchNotificationMessagesAsync(
            SearchNotificationMessageViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken));

        #endregion
    }
}