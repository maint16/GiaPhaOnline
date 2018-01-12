namespace Shared.ViewModels.PostReports
{
    public class InitiatePostReportViewModel
    {
        /// <summary>
        ///     Id of post which should be reported.
        /// </summary>
        public int PostIndex { get; set; }

        /// <summary>
        ///     Reason why the post should be reported.
        /// </summary>
        public string Reason { get; set; }
    }
}