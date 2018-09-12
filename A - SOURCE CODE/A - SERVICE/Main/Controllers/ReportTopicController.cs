using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDb.Interfaces;
using AppDb.Models.Entities;
using AppModel.Enumerations;
using AppModel.Enumerations.Order;
using AutoMapper;
using Main.Interfaces.Services;
using Main.ViewModels.Reply;
using Main.ViewModels.ReportTopic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Services;
using Shared.Models;
using Shared.Resources;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Main.Controllers
{
    [Route("api/report-topic")]
    public class ReportTopicController : ApiBaseController
    {
        #region Properties

        /// <summary>
        ///     Instance for accessing database.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        ///     Provide access to generic database functions.
        /// </summary>
        private readonly IRelationalDbService _databaseFunction;

        #endregion

        #region Constructures

        public ReportTopicController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ITimeService timeService,
            IRelationalDbService relationalDbService,
            IEncryptionService encryptionService,
            IIdentityService identityService) : base(unitOfWork, mapper, timeService,
            relationalDbService, identityService)
        {
            _unitOfWork = unitOfWork;
            _databaseFunction = relationalDbService;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Report a topic.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<IActionResult> ReportTopic([FromBody] AddReportTopicViewModel info)
        {
            #region Parameters validation

            if (info == null)
            {
                info = new AddReportTopicViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Find topic

            // Find topic.
            var topics = UnitOfWork.Topics.Search();
            topics = topics.Where(x => x.Id == info.TopicId && x.Status == ItemStatus.Active);

            // Get the first matched topic.
            var topic = await topics.FirstOrDefaultAsync();

            // Check whether topic exists or not.
            var bIsTopicAvailable = await topics.AnyAsync();
            if (!bIsTopicAvailable)
                return NotFound(new ApiResponse(HttpMessages.TopicNotFound));

            #endregion

            #region Report topic initialization

            // Find identity from request.
            var identity = IdentityService.GetProfile(HttpContext);

            // Report topic intialization.
            var reportTopic = new ReportTopic();
            reportTopic.OwnerId = topic.OwnerId;
            reportTopic.TopicId = info.TopicId;
            reportTopic.ReporterId = identity.Id;
            reportTopic.Reason = info.Reason;
            reportTopic.Status = ItemStatus.Active;
            reportTopic.CreatedTime = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);
            reportTopic.LastModifiedTime = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Insert report topic into system.
            UnitOfWork.ReportTopics.Insert(reportTopic);

            await UnitOfWork.CommitAsync();

            #endregion

            return Ok(reportTopic);
        }

        /// <summary>
        /// Edit reply by using specific information.
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPut("{topicId}")]
        public async Task<IActionResult> EditReply([FromRoute] int topicId, [FromBody] EditReportTopicViewModel info)
        {
            #region Parameters validation

            if (info == null)
            {
                info = new EditReportTopicViewModel();
                TryValidateModel(info);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            #endregion

            #region Find reply

            // Get request identity.
            var identity = IdentityService.GetProfile(HttpContext);

            // Get all report topic in database.
            var reportTopics = UnitOfWork.ReportTopics.Search();

            reportTopics = reportTopics.Where(x => x.TopicId == topicId && x.Status == ItemStatus.Active);

            // Get the first matched report topic.
            var reportTopic = await reportTopics.FirstOrDefaultAsync();
            if (reportTopic == null)
                return NotFound(new ApiResponse(HttpMessages.ReportTopicNotFound));

            #endregion

            #region Update report topic information

            // Check whether information has been updated or not.
            var bHasInformationChanged = false;

            // Reason is defined
            if (info.Reason != null && info.Reason != reportTopic.Reason)
            {
                reportTopic.Reason = info.Reason;
                bHasInformationChanged = true;
            }

            // Status is defined.
            if (info.Status != reportTopic.Status)
            {
                reportTopic.Status = info.Status;
                bHasInformationChanged = true;
            }

            if (bHasInformationChanged)
            {
                reportTopic.LastModifiedTime = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);

                // Commit changes to database.
                await UnitOfWork.CommitAsync();
            }

            #endregion

            return Ok(reportTopic);
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

            // Find identity in request.
            var identity = IdentityService.GetProfile(HttpContext);

            #region Search for information

            // Get all reply
            var reportTopics = _unitOfWork.ReportTopics.Search();

            // Topic Id have been defined.
            if (condition.TopicIds != null && condition.TopicIds.Count > 0)
            {
                condition.TopicIds = condition.TopicIds.Where(x => x > 0).ToList();
                if (condition.TopicIds != null && condition.TopicIds.Count > 0)
                {
                    reportTopics = reportTopics.Where(x => condition.TopicIds.Contains(x.TopicId));
                }
            }

            // Reporter id have been defined
            if (condition.ReporterIds != null && condition.ReporterIds.Count > 0)
            {
                condition.ReporterIds = condition.ReporterIds.Where(x => x > 0).ToList();
                if (condition.ReporterIds != null && condition.ReporterIds.Count > 0)
                {
                    reportTopics = reportTopics.Where(x => condition.ReporterIds.Contains(x.ReporterId));
                }
            }

            // Owner Id have been defined.
            if (condition.OwnerIds != null && condition.OwnerIds.Count > 0)
            {
                condition.OwnerIds = condition.OwnerIds.Where(x => x > 0).ToList();
                if (condition.OwnerIds != null && condition.OwnerIds.Count > 0)
                {
                    reportTopics = reportTopics.Where(x => condition.OwnerIds.Contains(x.OwnerId));
                }
            }

            // Content have been defined.
            if (condition.Reasons != null && condition.Reasons.Count > 0)
            {
                condition.Reasons = condition.Reasons.Where(x => !string.IsNullOrEmpty(x)).ToList();
                if (condition.Reasons != null && condition.Reasons.Count > 0)
                {
                    reportTopics = reportTopics.Where(x => condition.Reasons.Any(y => x.Reason.Contains(y)));
                }
            }

            // Search conditions which are based on roles.

            if (identity?.Role == AccountRole.Admin)
            {
                // Statuses have been defined.
                if (condition.Statuses != null && condition.Statuses.Count > 0)
                {
                    condition.Statuses =
                        condition.Statuses.Where(x => Enum.IsDefined(typeof(ItemStatus), x)).ToList();
                    if (condition.Statuses.Count > 0)
                        reportTopics = reportTopics.Where(x => condition.Statuses.Contains(x.Status));
                }
            }

            #endregion

            // Sort by properties.
            if (condition.Sort != null)
                reportTopics =
                    _databaseFunction.Sort(reportTopics, condition.Sort.Direction,
                        condition.Sort.Property);
            else
                reportTopics = _databaseFunction.Sort(reportTopics, SortDirection.Decending,
                    ReportTopicSort.CreatedTime);

            // Result initialization.
            var result = new SearchResult<IList<ReportTopic>>();
            result.Total = await reportTopics.CountAsync();
            result.Records = await _databaseFunction.Paginate(reportTopics, condition.Pagination).ToListAsync();

            return Ok(result);
        }

        #endregion
    }
}
