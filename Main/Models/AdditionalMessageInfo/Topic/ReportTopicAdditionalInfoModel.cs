namespace Main.Models.AdditionalMessageInfo.Topic
{
    public class ReportTopicAdditionalInfoModel
    {
        #region Properties

        public string TopicName { get; set; }

        public string ReporterName { get; set; }

        #endregion

        #region Constructor

        public ReportTopicAdditionalInfoModel()
        {
        }

        public ReportTopicAdditionalInfoModel(string topicName, string reporterName)
        {
            TopicName = topicName;
            ReporterName = reporterName;
        }

        #endregion
    }
}
