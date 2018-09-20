using System.ComponentModel.DataAnnotations;

namespace Main.ViewModels.ReportTopic
{
    public class AddReportTopicViewModel
    {
        #region Properties

        /// <summary>
        /// Id of topic
        /// </summary>
        [Required]
        public int TopicId { get; set; }

        /// <summary>
        /// Report reason
        /// </summary>
        [Required]
        public string Reason { get; set; }

        #endregion
    }
}
