using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppBusiness.Interfaces;
using AppBusiness.Interfaces.Domains;
using AppDb.Interfaces;
using AppShared.ViewModels.Reply;
using AutoMapper;
using Main.Constants;
using Main.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServiceShared.Authentications.ActionFilters;
using ServiceShared.Exceptions;
using ServiceShared.Interfaces.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Main.Controllers
{
    [Route("api/[controller]")]
    public class ReplyController : Controller
    {
        #region Constructors

        public ReplyController(
            IAppUnitOfWork unitOfWork,
            IMapper mapper,
            ITimeService timeService,
            IAppProfileService profileService,
            ISendMailService sendMailService,
            IEmailCacheService emailCacheService,
            ILogger<ReplyController> logger, IReplyDomain replyDomain)
        {
            _sendMailService = sendMailService;
            _emailCacheService = emailCacheService;
            _logger = logger;
            _replyDomain = replyDomain;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Send email service
        /// </summary>
        private readonly ISendMailService _sendMailService;

        /// <summary>
        ///     Email cache service.
        /// </summary>
        private readonly IEmailCacheService _emailCacheService;

        /// <summary>
        ///     Logging instance.
        /// </summary>
        private readonly ILogger _logger;

        private readonly IReplyDomain _replyDomain;

        #endregion

        #region Methods

        /// <summary>
        ///     Add reply to system.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<IActionResult> AddReply([FromBody] AddReplyViewModel model)
        {
            #region Parameters validation

            if (model == null)
            {
                model = new AddReplyViewModel();
                TryValidateModel(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            var topicReply = await _replyDomain.AddReplyAsync(model);
            return Ok(topicReply);
        }

        /// <summary>
        ///     Edit reply by using specific information.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditReply([FromRoute] int id, [FromBody] EditReplyViewModel info)
        {
            #region Parameters validation

            if (info == null)
            {
                info = new EditReplyViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            // Update reply information.
            var reply = await _replyDomain.EditReplyAsync(id, info);
            return Ok(reply);
        }

        /// <summary>
        ///     Delete a category.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("")]
        [Authorize(Policy = PolicyConstant.IsAdminPolicy)]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id)
        {
            var deleteReplyViewModel = new DeleteReplyViewModel
            {
                Id = id
            };

            await _replyDomain.DeleteReplyAsync(deleteReplyViewModel);

            return Ok();
        }

        /// <summary>
        ///     Load reply by using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("search")]
        [ByPassAuthorization]
        public async Task<IActionResult> LoadReplies([FromBody] SearchReplyViewModel condition)
        {
            #region Parameters validation

            if (condition == null)
            {
                condition = new SearchReplyViewModel();
                TryValidateModel(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            var loadTopicRepliesResult = await _replyDomain.SearchRepliesAsync(condition);
            return Ok(loadTopicRepliesResult);
        }

        #endregion
    }
}