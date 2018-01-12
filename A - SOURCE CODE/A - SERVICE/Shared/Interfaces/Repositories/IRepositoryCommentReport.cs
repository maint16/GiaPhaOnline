using System.Linq;
using SystemDatabase.Models.Entities;
using Shared.ViewModels.CommentReports;

namespace Shared.Interfaces.Repositories
{
    public interface IRepositoryCommentReport : IParentRepository<CommentReport>
    {
        /// <summary>
        ///     Search comment report by using specific conditions.
        /// </summary>
        /// <param name="commentReports"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        IQueryable<CommentReport> Search(IQueryable<CommentReport> commentReports,
            SearchCommentReportViewModel conditions);
    }
}