namespace AuthenticationModel.Models
{
    public class Pagination
    {
        /// <summary>
        ///     Id of result page.
        ///     Min: 0
        ///     Max: (infinite)
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        ///     Maximum records can be displayed per page.
        /// </summary>
        public int Records { get; set; }
    }
}
