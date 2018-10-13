namespace Shared.Models
{
    public class CategoryPostCounterModel
    {
        #region Properties

        /// <summary>
        ///     Id of category.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        ///     Total posts number of the current category.
        /// </summary>
        public int TotalPosts { get; set; }

        #endregion
    }
}