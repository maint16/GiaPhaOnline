namespace Shared.ViewModels.Posts
{
    public class InitiatePostViewModel
    {
        /// <summary>
        /// Id of category which post should belong to.
        /// </summary>
        public int CategoryIndex { get; set; }

        /// <summary>
        ///     Title of post.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Body of post.
        /// </summary>
        public string Body { get; set; }
    }
}