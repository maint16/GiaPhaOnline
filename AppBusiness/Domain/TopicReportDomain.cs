using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AppBusiness.Interfaces;
using AppBusiness.Interfaces.Domains;
using AppDb.Interfaces;
using AppDb.Models.Entities;
using AppShared.Enumerations;
using AppShared.Enumerations.Order;
using AppShared.Models;
using AppShared.Resources;
using AppShared.ViewModels.ReportTopic;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ServiceShared.Exceptions;
using ServiceShared.Interfaces.Services;

namespace AppBusiness.Domain
{
    public class TopicReportDomain : ITopicReportDomain
    {
        #region Constructors

        public TopicReportDomain(IUnitOfWork unitOfWork, ITimeService timeService,
            IHttpContextAccessor httpContextAccessor, IProfileService identityService,
            IRelationalDbService relationalDbService)
        {
            _unitOfWork = unitOfWork;
            _timeService = timeService;
            _httpContext = httpContextAccessor.HttpContext;
            _identityService = identityService;
            _relationalDbService = relationalDbService;
        }

        #endregion

        #region Properties

        private readonly IUnitOfWork _unitOfWork;

        private readonly ITimeService _timeService;

        private readonly HttpContext _httpContext;

        private readonly IProfileService _identityService;

        private readonly IRelationalDbService _relationalDbService;

        #endregion

        #region Methods

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<ReportTopic> AddTopicReportAsync(AddReportTopicViewModel model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Find topic.
            var topics = _unitOfWork.Topics.Search();
            topics = topics.Where(x => x.Id == model.TopicId && x.Status == ItemStatus.Active);

            // Get the first matched topic.
            var topic = await topics.FirstOrDefaultAsync(cancellationToken);

            if (topic == null)
                throw new ApiException(HttpMessages.TopicNotFound, HttpStatusCode.NotFound);

            // Find identity from request.
            var profile = _identityService.GetProfile();

            // Report topic intialization.
            var reportTopic = new ReportTopic();
            reportTopic.OwnerId = topic.OwnerId;
            reportTopic.TopicId = model.TopicId;
            reportTopic.ReporterId = profile.Id;
            reportTopic.Reason = model.Reason;
            reportTopic.Status = ItemStatus.Active;
            reportTopic.CreatedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);
            reportTopic.LastModifiedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Insert report topic into system.
            _unitOfWork.ReportTopics.Insert(reportTopic);

            await _unitOfWork.CommitAsync(cancellationToken);

            return reportTopic;
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<ReportTopic> EditTopicReportAsync(int id, EditReportTopicViewModel model,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Get request identity.
            var profile = _identityService.GetProfile();

            // Get all report topic in database.
            var reportTopics = _unitOfWork.ReportTopics.Search();
            reportTopics = reportTopics.Where(x => x.TopicId == id && x.Status == ItemStatus.Active);

            // Get the first matched report topic.
            var reportTopic = await reportTopics.FirstOrDefaultAsync(cancellationToken);
            if (reportTopic == null)
                throw new ApiException(HttpMessages.ReportTopicNotFound, HttpStatusCode.NotFound);

            // Check whether information has been updated or not.
            var bHasInformationChanged = false;

            // Reason is defined
            if (model.Reason != null && model.Reason != reportTopic.Reason)
            {
                reportTopic.Reason = model.Reason;
                bHasInformationChanged = true;
            }

            // Status is defined.
            if (model.Status != reportTopic.Status)
            {
                reportTopic.Status = model.Status;
                bHasInformationChanged = true;
            }

            if (!bHasInformationChanged)
                throw new NotModifiedException();

            reportTopic.LastModifiedTime = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

            // Commit changes to database.
            await _unitOfWork.CommitAsync(cancellationToken);
            return reportTopic;
        }

        /// <summary>
        ///     <inheritdoc />
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<SearchResult<IList<ReportTopic>>> SearchTopicReportsAsync(
            SearchReportTopicViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Get topic reports.
            var topicReports = GetTopicReports(condition);

            // Sort by properties.
            if (condition.Sort != null)
                topicReports =
                    _relationalDbService.Sort(topicReports, condition.Sort.Direction,
                        condition.Sort.Property);
            else
                topicReports = _relationalDbService.Sort(topicReports, SortDirection.Decending,
                    ReportTopicSort.CreatedTime);

            // Result initialization.
            var loadTopicReportResult = new SearchResult<IList<ReportTopic>>();
            loadTopicReportResult.Total = await topicReports.CountAsync(cancellationToken);
            loadTopicReportResult.Records = await _relationalDbService.Paginate(topicReports, condition.Pagination)
                .ToListAsync(cancellationToken);
            return loadTopicReportResult;
        }

        /// <summary>
        ///     Get topic reports using specific conditions.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected virtual IQueryable<ReportTopic> GetTopicReports(SearchReportTopicViewModel condition)
        {
            // Find identity in request.
            var profile = _identityService.GetProfile();

            // Whether user is admin or not.
            var bIsUserAdmin = profile != null && profile.Role == UserRole.Admin;

            // Get all reply
            var reportTopics = _unitOfWork.ReportTopics.Search();

            // Topic Id have been defined.
            var topicIds = condition.TopicIds;
            if (topicIds != null && topicIds.Count > 0)
            {
                topicIds = topicIds.Where(x => x > 0).ToHashSet();
                if (topicIds != null && topicIds.Count > 0)
                    reportTopics = reportTopics.Where(x => topicIds.Contains(x.TopicId));
            }

            // Reporter id have been defined
            var reporterIds = condition.ReporterIds;
            if (reporterIds != null && reporterIds.Count > 0)
            {
                reporterIds = reporterIds.Where(x => x > 0).ToHashSet();
                if (reporterIds != null && reporterIds.Count > 0)
                    reportTopics = reportTopics.Where(x => reporterIds.Contains(x.ReporterId));
            }


            if (bIsUserAdmin)
            {
                // Owner id has been defined.
                var ownerIds = condition.OwnerIds;
                if (ownerIds != null && ownerIds.Count > 0)
                {
                    ownerIds = ownerIds.Where(x => x > 0).ToHashSet();
                    if (ownerIds != null && ownerIds.Count > 0)
                        reportTopics = reportTopics.Where(x => ownerIds.Contains(x.OwnerId));
                }

                // Owner statuses has been defined.
                var statuses = condition.Statuses;
                if (statuses != null && statuses.Count > 0)
                {
                    statuses =
                        statuses.Where(x => Enum.IsDefined(typeof(ItemStatus), x)).ToHashSet();
                    if (statuses.Count > 0)
                        reportTopics = reportTopics.Where(x => statuses.Contains(x.Status));
                }
            }
            else
            {
                reportTopics = reportTopics.Where(x => x.OwnerId == profile.Id);
            }


            // Content have been defined.
            var reasons = condition.Reasons;
            if (reasons != null && reasons.Count > 0)
            {
                reasons = reasons.Where(x => !string.IsNullOrEmpty(x)).ToHashSet();
                if (reasons != null && reasons.Count > 0)
                    reportTopics = reportTopics.Where(x => reasons.Any(y => x.Reason.Contains(y)));
            }

            return reportTopics;
        }

        #endregion
    }
}