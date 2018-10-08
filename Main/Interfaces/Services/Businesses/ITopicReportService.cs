﻿using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppDb.Models.Entities;
using Main.ViewModels.ReportTopic;
using Shared.Models;

namespace Main.Services.Businesses
{
    public interface ITopicReportService
    {
        #region Methods

        /// <summary>
        /// Add topic report asynchronously.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ReportTopic> AddTopicReportAsync(AddReportTopicViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Edit topic report asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ReportTopic> EditTopicReportAsync(int id, EditReportTopicViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Search for topic reports using specific conditions.
        /// </summary>
        Task<SearchResult<IList<ReportTopic>>> SearchTopicReportsAsync(SearchReportTopicViewModel condition,
            CancellationToken cancellationToken = default(CancellationToken));

        #endregion
    }
}