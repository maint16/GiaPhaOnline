using System.Threading;
using System.Threading.Tasks;
using AppBusiness.Interfaces;
using AppBusiness.Interfaces.Domains;
using Microsoft.AspNetCore.Mvc;
using ServiceShared.Models;
using Shared.Resources;
using Shared.ViewModels.NotificationMessage;

namespace Main.Controllers
{
    [Route("api/notification-message")]
    public class NotificationMessageController : Controller
    {
        #region Properties

        /// <summary>
        /// Notification message business handler.
        /// </summary>
        private readonly INotificationMessageDomain _notificationMessageDomain;

        
        #endregion

        #region Constructor

        public NotificationMessageController(INotificationMessageDomain notificationMessageDomain)
        {
            _notificationMessageDomain = notificationMessageDomain;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get notification message using specific condition.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetNotificationMessage([FromRoute] GetNotificationMessageViewModel model)
        {
            if (model == null)
            {
                model = new GetNotificationMessageViewModel();
                TryValidateModel(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var notificationMessage = await _notificationMessageDomain.GetNotificationMessageUsingId(model.Id);
            if (notificationMessage == null)
                return NotFound(new ApiResponse(HttpMessages.NotificationMessageNotFound));

            return Ok(notificationMessage);
        }

        /// <summary>
        /// Search for notification messages.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("search")]
        public virtual async Task<IActionResult> SearchNotificationMessage(
            [FromBody] SearchNotificationMessageViewModel condition)
        {
            if (condition == null)
            {
                condition = new SearchNotificationMessageViewModel();
                TryValidateModel(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var loadNotificationMessagesResult =
                await _notificationMessageDomain.SearchNotificationMessagesAsync(condition, CancellationToken.None);

            return Ok(loadNotificationMessagesResult);
        }

        /// <summary>
        /// Mark a notification as seen.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("mark-as-seen/{id}")]
        public virtual async Task<IActionResult> MarkMessageAsSeen([FromRoute] GetNotificationMessageViewModel model)
        {
            if (model == null)
            {
                model = new GetNotificationMessageViewModel();
                TryValidateModel(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var notificationMessage = await _notificationMessageDomain.MarkNotificationMessageAsSeen(model.Id);
            if (notificationMessage == null)
                return NotFound(new ApiResponse(HttpMessages.NotificationMessageNotFound));

            return Ok(notificationMessage);
        }
        #endregion
    }
}