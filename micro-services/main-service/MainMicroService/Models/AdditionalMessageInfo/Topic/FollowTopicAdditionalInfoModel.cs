namespace MainMicroService.Models.AdditionalMessageInfo.Topic
{
    public class FollowTopicAdditionalInfoModel
    {
        #region Properties

        public string TopicName { get; set; }

        public string FollowerName { get; set; }

        #endregion

        #region Constructor

        public FollowTopicAdditionalInfoModel()
        {
        }

        public FollowTopicAdditionalInfoModel(string topicName, string followerName)
        {
            TopicName = topicName;
            FollowerName = followerName;
        }

        #endregion
    }
}