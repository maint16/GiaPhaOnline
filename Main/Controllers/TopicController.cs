﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppDb.Interfaces;
using AppModel.Enumerations;
using AutoMapper;
using Main.Constants;
using Main.Interfaces.Services;
using Main.Interfaces.Services.Businesses;
using Main.ViewModels.Topic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Interfaces.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Main.Controllers
{
    [Route("api/[controller]")]
    public class TopicController : ApiBaseController
    {
        #region Constructors

        public TopicController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ITimeService timeService,
            IRelationalDbService relationalDbService,
            IEncryptionService encryptionService,
            IIdentityService identityService,
            ISendMailService sendMailService,
            IEmailCacheService emailCacheService,
            ILogger<TopicController> logger, ITopicService topicService) : base(unitOfWork, mapper, timeService,
            relationalDbService, identityService)
        {
            _sendMailService = sendMailService;
            _emailCacheService = emailCacheService;
            _logger = logger;
            _topicService = topicService;
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

        private readonly ITopicService _topicService;

        #endregion

        #region Methods

        /// <summary>
        ///     Add topic to system.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<IActionResult> AddTopic([FromBody] AddTopicViewModel model)
        {
            if (model == null)
            {
                model = new AddTopicViewModel();
                TryValidateModel(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var topic = await _topicService.AddTopicAsync(model, CancellationToken.None);

            return Ok(topic);
        }

        /// <summary>
        ///     Edit topic by using specific information.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditTopic([FromRoute] int id, [FromBody] EditTopicViewModel model)
        {
            #region Parameters validation

            if (model == null)
            {
                model = new EditTopicViewModel();
                TryValidateModel(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            // Update topic information.
            var topic = await _topicService.EditTopicAsync(id, model);

            if (topic.Status != ItemStatus.Disabled)
                return Ok(topic);

            var users = UnitOfWork.Accounts.Search();
            users = users.Where(x => x.Id == topic.OwnerId);
            var user = await users.FirstOrDefaultAsync();

            if (user != null)
            {
                var emailTemplate = _emailCacheService.Read(EmailTemplateConstant.DeleteTopic);
                if (emailTemplate != null)
                {
                    await _sendMailService.SendAsync(new HashSet<string> {user.Email}, null, null,
                        emailTemplate.Subject,
                        emailTemplate.Content, true, CancellationToken.None);

                    _logger.LogInformation($"Sent message to {user.Email} with subject {emailTemplate.Subject}");
                }
            }

            return Ok(topic);
        }

        /// <summary>
        ///     Load topic by using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("search")]
        public async Task<IActionResult> LoadTopics([FromBody] SearchTopicViewModel condition)
        {
            #region Parameters validation

            if (condition == null)
            {
                condition = new SearchTopicViewModel();
                TryValidateModel(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            var loadTopicsResult = await _topicService.SearchTopicsAsync(condition, CancellationToken.None);
            return Ok(loadTopicsResult);
        }

        #endregion
    }
}