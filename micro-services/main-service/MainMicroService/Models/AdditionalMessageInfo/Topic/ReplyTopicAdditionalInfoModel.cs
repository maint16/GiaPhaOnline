namespace MainMicroService.Models.AdditionalMessageInfo.Topic
{
    public class ReplyTopicAdditionalInfoModel
    {
        #region Properties

        public string TopicName { get; set; }

        public string ReplierName { get; set; }

        #endregion

        #region Constructor

        public ReplyTopicAdditionalInfoModel()
        {
        }

        public ReplyTopicAdditionalInfoModel(string topicName, string replierName)
        {
            TopicName = topicName;
            ReplierName = replierName;
        }

        #endregion
    }
}