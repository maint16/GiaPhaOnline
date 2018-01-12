namespace Shared.ViewModels.Comments
{
    public class InitiateCommentViewModel
    {
        /// <summary>
        ///     Id of post which comment should belong to.
        /// </summary>
        public int PostIndex { get; set; }

        /// <summary>
        ///     Comment content.
        /// </summary>
        public string Content { get; set; }
    }
}