using System;
using System.Threading.Tasks;
using AppBusiness.Interfaces;
using AppBusiness.Interfaces.Domains;
using AppDb.Interfaces;
using AppShared.ViewModels.ReportTopic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ServiceShared.Exceptions;
using ServiceShared.Interfaces.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Main.Controllers
{
    [Route("api/report-topic")]
    public class ReportTopicController : ApiBaseController
    {
        #region Properties

        private readonly ITopicReportDomain _topicReportDomain;

        #endregion

        #region Constructures

        public ReportTopicController(
            IAppUnitOfWork unitOfWork,
            IMapper mapper,
            ITimeService timeService,
            IBaseRelationalDbService relationalDbService,
            IEncryptionService encryptionService,
            IAppProfileService profileService, ITopicReportDomain topicReportDomain) : base(unitOfWork, mapper,
            timeService,
            relationalDbService, profileService)
        {
            _topicReportDomain = topicReportDomain;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Report a topic.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<IActionResult> AddReportTopic([FromBody] AddReportTopicViewModel model)
        {
            if (model == null)
            {
                model = new AddReportTopicViewModel();
                TryValidateModel(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var topicReport = await _topicReportDomain.AddTopicReportAsync(model);
            return Ok(topicReport);
        }

        /// <summary>
        ///     Edit reply by using specific information.
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{topicId}")]
        public async Task<IActionResult> EditTopicReport([FromRoute] int topicId,
            [FromBody] EditReportTopicViewModel model)
        {
            if (model == null)
            {
                model = new EditReportTopicViewModel();
                TryValidateModel(model);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var topicReport = await _topicReportDomain.EditTopicReportAsync(topicId, model);
                return Ok(topicReport);
            }
            catch (Exception exception)
            {
                if (!(exception is NotModifiedException))
                    throw;

                return Ok();
            }
        }

        /// <summary>
        ///     Search topic report by using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("search")]
        public async Task<IActionResult> LoadReportTopics([FromBody] SearchReportTopicViewModel condition)
        {
            #region Parameters validation

            if (condition == null)
            {
                condition = new SearchReportTopicViewModel();
                TryValidateModel(condition);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            var loadTopicReportsResult = await _topicReportDomain.SearchTopicReportsAsync(condition);
            return Ok(loadTopicReportsResult);
        }

        #endregion
    }
}