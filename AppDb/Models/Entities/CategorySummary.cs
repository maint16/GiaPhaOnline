using Newtonsoft.Json;

namespace AppDb.Models.Entities
{
    public class CategorySummary
    {
        #region Properties

        public int CategoryId { get; set; }

        public int TotalPost { get; set; }

        public int TotalFollower { get; set; }

        public int LastTopicId { get; set; }

        public string LastTopicTitle { get; set; }

        public double LastTopicCreatedTime { get; set; }

        #endregion

        #region Relationships

        [JsonIgnore]
        public virtual Category Category { get; set; }
        
        #endregion

        #region Constructors

        public CategorySummary()
        {
        }

        public CategorySummary(int categoryId, int totalPost, int totalFollower)
        {
            CategoryId = categoryId;
            TotalPost = totalPost;
            TotalFollower = totalFollower;
        }

        public CategorySummary(int categoryId, int totalPost, int totalFollower, int lastTopicId, string lastTopicTitle,
            double lastTopicCreatedTime) : this(categoryId, totalPost, totalFollower)
        {
            CategoryId = categoryId;
            TotalPost = totalPost;
            TotalFollower = totalFollower;
            LastTopicId = lastTopicId;
            LastTopicTitle = lastTopicTitle;
            LastTopicCreatedTime = lastTopicCreatedTime;
        }

        #endregion
    }
}