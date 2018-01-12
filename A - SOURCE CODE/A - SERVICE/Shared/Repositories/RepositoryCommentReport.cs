using System;
using System.Linq;
using SystemDatabase.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Enumerations;
using Shared.Interfaces.Repositories;
using Shared.ViewModels.CommentReports;

namespace Shared.Repositories
{
    public class RepositoryCommentReport : ParentRepository<CommentReport>, IRepositoryCommentReport
    {
        #region Constructors

        /// <summary>
        ///     Initiate repository of comment reports.
        /// </summary>
        /// <param name="dbContext"></param>
        public RepositoryCommentReport(
            DbContext dbContext) : base(dbContext)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Search comment reports by using specific conditions.
        /// </summary>
        /// <param name="commentReports"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public IQueryable<CommentReport> Search(IQueryable<CommentReport> commentReports,
            SearchCommentReportViewModel conditions)
        {
            // Comment index is specified.
            if (conditions.CommentIndex != null)
                commentReports = commentReports.Where(x => x.CommentId == conditions.CommentIndex.Value);

            // Comment owner is specified.
            if (conditions.CommentOwnerIndex != null)
                commentReports = commentReports.Where(x => x.OwnerId == conditions.CommentOwnerIndex.Value);

            // Reporter index is specified.
            if (conditions.CommentReporterIndex != null)
                commentReports =
                    commentReports.Where(x => x.ReporterId == conditions.CommentReporterIndex.Value);

            // Comment body is specified.
            if (conditions.Body != null && !string.IsNullOrEmpty(conditions.Body.Value))
            {
                var szBody = conditions.Body;
                switch (szBody.Mode)
                {
                    case TextSearchMode.Contain:
                        commentReports = commentReports.Where(x => x.Body.Contains(szBody.Value));
                        break;
                    case TextSearchMode.Equal:
                        commentReports = commentReports.Where(x => x.Body.Equals(szBody.Value));
                        break;
                    case TextSearchMode.EqualIgnoreCase:
                        commentReports =
                            commentReports.Where(
                                x => x.Body.Equals(szBody.Value, StringComparison.CurrentCultureIgnoreCase));
                        break;
                    case TextSearchMode.StartsWith:
                        commentReports = commentReports.Where(x => x.Body.StartsWith(szBody.Value));
                        break;
                    case TextSearchMode.StartsWithIgnoreCase:
                        commentReports =
                            commentReports.Where(
                                x => x.Body.StartsWith(szBody.Value, StringComparison.CurrentCultureIgnoreCase));
                        break;
                    case TextSearchMode.EndsWith:
                        commentReports = commentReports.Where(x => x.Body.EndsWith(szBody.Value));
                        break;
                    case TextSearchMode.EndsWithIgnoreCase:
                        commentReports =
                            commentReports.Where(
                                x => x.Body.EndsWith(szBody.Value, StringComparison.CurrentCultureIgnoreCase));
                        break;
                    default:
                        commentReports = commentReports.Where(x => x.Body.ToLower().Contains(szBody.Value.ToLower()));
                        break;
                }
                return commentReports;
            }

            // Comment reason is specified.
            if (conditions.Reason != null && !string.IsNullOrEmpty(conditions.Reason.Value))
            {
                var szReason = conditions.Reason;
                switch (szReason.Mode)
                {
                    case TextSearchMode.Contain:
                        commentReports = commentReports.Where(x => x.Reason.Contains(szReason.Value));
                        break;
                    case TextSearchMode.Equal:
                        commentReports = commentReports.Where(x => x.Reason.Equals(szReason.Value));
                        break;
                    case TextSearchMode.EqualIgnoreCase:
                        commentReports =
                            commentReports.Where(
                                x => x.Reason.Equals(szReason.Value, StringComparison.CurrentCultureIgnoreCase));
                        break;
                    case TextSearchMode.StartsWith:
                        commentReports = commentReports.Where(x => x.Reason.StartsWith(szReason.Value));
                        break;
                    case TextSearchMode.StartsWithIgnoreCase:
                        commentReports =
                            commentReports.Where(
                                x => x.Reason.StartsWith(szReason.Value, StringComparison.CurrentCultureIgnoreCase));
                        break;
                    case TextSearchMode.EndsWith:
                        commentReports = commentReports.Where(x => x.Reason.EndsWith(szReason.Value));
                        break;
                    case TextSearchMode.EndsWithIgnoreCase:
                        commentReports =
                            commentReports.Where(
                                x => x.Reason.EndsWith(szReason.Value, StringComparison.CurrentCultureIgnoreCase));
                        break;
                    default:
                        commentReports = commentReports.Where(x => x.Reason.ToLower().Contains(szReason.Value.ToLower()));
                        break;
                }
            }

            //// CreatedTime is specified.
            //if (conditions.Created != null)
            //{
            //    // Comment created time.
            //    var created = conditions.Created;

            //    // From is defined.
            //    if (created.From != null)
            //        commentReports = commentReports.Where(x => x.Created >= created.From.Value);

            //    // To is defined.
            //    if (created.To != null)
            //        commentReports = commentReports.Where(x => x.Created <= created.To.Value);
            //}

            return commentReports;
        }

        #endregion
    }
}