using System.Linq;
using SystemDatabase.Models.Entities;
using Shared.ViewModels.PostReports;

namespace Shared.Interfaces.Repositories
{
    public interface IRepositoryPostReport : IParentRepository<PostReport>
    {
        /// <summary>
        ///     Search post reports by using specific conditions.
        /// </summary>
        /// <param name="postReports"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        IQueryable<PostReport> Search(IQueryable<PostReport> postReports,
            SearchPostReportViewModel conditions);
    }
}