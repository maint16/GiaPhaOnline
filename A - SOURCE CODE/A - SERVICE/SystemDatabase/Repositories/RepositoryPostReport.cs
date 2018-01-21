using SystemDatabase.Interfaces.Repositories;
using SystemDatabase.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace SystemDatabase.Repositories
{
    public class RepositoryPostReport : ParentRepository<PostReport>, IRepositoryPostReport
    {
        #region Constructors

        /// <summary>
        ///     Initiate repository with inversion of control.
        /// </summary>
        /// <param name="dbContext"></param>
        public RepositoryPostReport(
            DbContext dbContext) : base(dbContext)
        {
        }

        #endregion

        #region Methods

        ///// <summary>
        /////     Search posts by using specific conditions.
        ///// </summary>
        ///// <param name="postReports"></param>
        ///// <param name="conditions"></param>
        ///// <returns></returns>
        //public IQueryable<PostReport> Search(IQueryable<PostReport> postReports,
        //    SearchPostReportViewModel conditions)
        //{
        //    // Owner index is specified.
        //    if (conditions.PostOwnerIndex != null)
        //        postReports = postReports.Where(x => x.OwnerId == conditions.PostOwnerIndex.Value);

        //    // Reporter index is specified.
        //    if (conditions.PostReporterIndex != null)
        //        postReports = postReports.Where(x => x.ReporterId == conditions.PostReporterIndex.Value);

        //    // Body of post.
        //    if (conditions.Body != null && !string.IsNullOrEmpty(conditions.Body.Value))
        //    {
        //        var szBody = conditions.Body;
        //        switch (szBody.Mode)
        //        {
        //            case TextSearchMode.Contain:
        //                postReports = postReports.Where(x => x.Body.Contains(szBody.Value));
        //                break;
        //            case TextSearchMode.Equal:
        //                postReports = postReports.Where(x => x.Body.Equals(szBody.Value));
        //                break;
        //            case TextSearchMode.EqualIgnoreCase:
        //                postReports =
        //                    postReports.Where(
        //                        x => x.Body.Equals(szBody.Value, StringComparison.CurrentCultureIgnoreCase));
        //                break;
        //            case TextSearchMode.StartsWith:
        //                postReports = postReports.Where(x => x.Body.StartsWith(szBody.Value));
        //                break;
        //            case TextSearchMode.StartsWithIgnoreCase:
        //                postReports =
        //                    postReports.Where(
        //                        x => x.Body.StartsWith(szBody.Value, StringComparison.CurrentCultureIgnoreCase));
        //                break;
        //            case TextSearchMode.EndsWith:
        //                postReports = postReports.Where(x => x.Body.EndsWith(szBody.Value));
        //                break;
        //            case TextSearchMode.EndsWithIgnoreCase:
        //                postReports =
        //                    postReports.Where(
        //                        x => x.Body.EndsWith(szBody.Value, StringComparison.CurrentCultureIgnoreCase));
        //                break;
        //            default:
        //                postReports = postReports.Where(x => x.Body.ToLower().Contains(szBody.Value.ToLower()));
        //                break;
        //        }
        //    }
        //    // Reason is specified.
        //    if (conditions.Reason != null && !string.IsNullOrEmpty(conditions.Reason.Value))
        //    {
        //        var szReason = conditions.Reason;
        //        switch (szReason.Mode)
        //        {
        //            case TextSearchMode.Contain:
        //                postReports = postReports.Where(x => x.Reason.Contains(szReason.Value));
        //                break;
        //            case TextSearchMode.Equal:
        //                postReports = postReports.Where(x => x.Reason.Equals(szReason.Value));
        //                break;
        //            case TextSearchMode.EqualIgnoreCase:
        //                postReports =
        //                    postReports.Where(
        //                        x => x.Reason.Equals(szReason.Value, StringComparison.CurrentCultureIgnoreCase));
        //                break;
        //            case TextSearchMode.StartsWith:
        //                postReports = postReports.Where(x => x.Reason.StartsWith(szReason.Value));
        //                break;
        //            case TextSearchMode.StartsWithIgnoreCase:
        //                postReports =
        //                    postReports.Where(
        //                        x => x.Reason.StartsWith(szReason.Value, StringComparison.CurrentCultureIgnoreCase));
        //                break;
        //            case TextSearchMode.EndsWith:
        //                postReports = postReports.Where(x => x.Reason.EndsWith(szReason.Value));
        //                break;
        //            case TextSearchMode.EndsWithIgnoreCase:
        //                postReports =
        //                    postReports.Where(
        //                        x => x.Reason.EndsWith(szReason.Value, StringComparison.CurrentCultureIgnoreCase));
        //                break;
        //            default:
        //                postReports = postReports.Where(x => x.Reason.ToLower().Contains(szReason.Value.ToLower()));
        //                break;
        //        }
        //    }

        //    //// CreatedTime is specified.
        //    //if (conditions.CreatedTime != null)
        //    //{
        //    //    var created = conditions.CreatedTime;

        //    //    // From is defined.
        //    //    if (created.From != null)
        //    //        postReports = postReports.Where(x => x.Created >= created.From.Value);

        //    //    // To is defined.
        //    //    if (created.To != null)
        //    //        postReports = postReports.Where(x => x.Created <= created.To.Value);
        //    //}

        //    return postReports;
        //}

        #endregion
    }
}