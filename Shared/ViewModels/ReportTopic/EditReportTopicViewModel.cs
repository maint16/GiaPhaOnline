using Shared.Enumerations;

namespace Shared.ViewModels.ReportTopic
{
    public class EditReportTopicViewModel
    {
        #region Methods

        /// <summary>
        ///     Report reason
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        ///     Status of report topic.
        /// </summary>
        public ItemStatus Status { get; set; }

        #endregion
    }
}