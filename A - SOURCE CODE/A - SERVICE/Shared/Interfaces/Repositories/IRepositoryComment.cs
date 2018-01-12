using System.Linq;
using SystemDatabase.Models.Entities;
using Shared.ViewModels.Comments;

namespace Shared.Interfaces.Repositories
{
    public interface IRepositoryComment : IParentRepository<Comment>
    {
        /// <summary>
        ///     Search comments by using specific conditions.
        /// </summary>
        /// <param name="comments"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        IQueryable<Comment> Search(IQueryable<Comment> comments, SearchCommentViewModel conditions);
    }
}