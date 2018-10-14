using Newtonsoft.Json;

namespace AppDb.Models.Entities
{
    public class TopicSummary
    {
        #region Properties

        public int TopicId { get; set; }

        public int TotalFollower { get; set; }

        public int TotalReply { get; set; }

        #endregion

        #region Relationship

        [JsonIgnore]
        public Topic Topic { get; set; }

        #endregion

        #region Constructor

        public TopicSummary()
        {
        }

        public TopicSummary(int topicId, int totalFollower, int totalReply)
        {
            TopicId = topicId;
            TotalFollower = totalFollower;
            TotalReply = totalReply;
        }

        #endregion
    }
}