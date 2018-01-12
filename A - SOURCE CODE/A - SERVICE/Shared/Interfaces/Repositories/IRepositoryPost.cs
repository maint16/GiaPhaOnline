using System.Linq;
using SystemDatabase.Models.Entities;
using Shared.ViewModels.Posts;

namespace Shared.Interfaces.Repositories
{
    public interface IRepositoryPost : IParentRepository<Post>
    {
        #region Properties

        /// <summary>
        ///     Search posts by using specific conditions.
        /// </summary>
        /// <param name="posts"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        IQueryable<Post> Search(IQueryable<Post> posts, SearchPostViewModel conditions);

        #endregion
    }
}