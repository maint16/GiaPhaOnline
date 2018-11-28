namespace MainMicroService.Models.AdditionalMessageInfo.Topic
{
    public class AddTopicAdditionalInfoModel
    {
        #region Properties

        public string TopicName { get; set; }

        public string CreatorName { get; set; }

        #endregion

        #region Constructor

        public AddTopicAdditionalInfoModel()
        {
        }

        public AddTopicAdditionalInfoModel(string topicName, string creatorName)
        {
            TopicName = topicName;
            CreatorName = creatorName;
        }

        #endregion
    }
}