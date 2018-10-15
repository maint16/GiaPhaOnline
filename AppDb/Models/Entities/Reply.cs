using Shared.Enumerations;

namespace AppDb.Models.Entities
{
    public class Reply
    {
        #region Properties

        /// <summary>
        ///     Id of comment (Auto incremented)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Who wrote the comment.
        /// </summary>
        public int OwnerId { get; set; }

        /// <summary>
        ///     Which post this comment belongs to.
        /// </summary>
        public int TopicId { get; set; }

        /// <summary>
        ///     Category that reply belongs to.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        ///     Category group that reply belongs to.
        /// </summary>
        public int CategoryGroupId { get; set; }

        /// <summary>
        ///     Comment content.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        ///     Status of comment.
        /// </summary>
        public ItemStatus Status { get; set; }

        /// <summary>
        ///     When was the comment created.
        /// </summary>
        public double CreatedTime { get; set; }

        /// <summary>
        ///     When the comment was lastly modified.
        /// </summary>
        public double? LastModifiedTime { get; set; }

        #endregion

        #region Relationships

        /// <summary>
        ///     One comment can only be initiated by one account.
        /// </summary>
        public User Owner { get; set; }

        /// <summary>
        ///     One reply can only belong to one topic.
        /// </summary>
        public Topic Topic { get; set; }

        #endregion

        #region Constructors

        public Reply()
        {
        }

        public Reply(int id, int ownerId, int topicId, int categoryId, int categoryGroupId, string content,
            ItemStatus status, double createdTime, double? lastModifiedTime)
        {
            Id = id;
            OwnerId = ownerId;
            TopicId = topicId;
            CategoryId = categoryId;
            CategoryGroupId = categoryGroupId;
            Content = content;
            Status = status;
            CreatedTime = createdTime;
            LastModifiedTime = lastModifiedTime;
        }

        #endregion
    }
}