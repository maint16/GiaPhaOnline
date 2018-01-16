using System.Linq;
using Shared.Interfaces.Repositories;
using Shared.ViewModels.Posts;
using Shared.Enumerations;
using System;
using SystemDatabase.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Shared.Repositories
{
    public class RepositoryPost : ParentRepository<Post>, IRepositoryPost
    {
        #region Constructors

        /// <summary>
        ///     Initiate repository with inversion of control.
        /// </summary>
        public RepositoryPost(
            DbContext dbContext) : base(dbContext)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Search posts by using specific conditions.
        /// </summary>
        /// <param name="posts"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public IQueryable<Post> Search(IQueryable<Post> posts, SearchPostViewModel conditions)
        {
            // Id is specified.
            if (conditions.Id != null)
                posts = posts.Where(x => x.Id == conditions.Id.Value);

            // Owner index is specified.
            if (conditions.OwnerId != null)
                posts = posts.Where(x => x.OwnerId == conditions.OwnerId.Value);
            
            // Title is specified.
            if (conditions.Title != null && !string.IsNullOrEmpty(conditions.Title.Value))
            {
                var szTitle = conditions.Title;
                switch (szTitle.Mode)
                {
                    case TextSearchMode.Contain:
                        posts = posts.Where(x => x.Title.Contains(szTitle.Value));
                        break;
                    case TextSearchMode.Equal:
                        posts = posts.Where(x => x.Title.Equals(szTitle.Value));
                        break;
                    case TextSearchMode.EqualIgnoreCase:
                        posts =
                            posts.Where(x => x.Title.Equals(szTitle.Value, StringComparison.CurrentCultureIgnoreCase));
                        break;
                    case TextSearchMode.StartsWith:
                        posts = posts.Where(x => x.Title.StartsWith(szTitle.Value));
                        break;
                    case TextSearchMode.StartsWithIgnoreCase:
                        posts =
                            posts.Where(
                                x => x.Title.StartsWith(szTitle.Value, StringComparison.CurrentCultureIgnoreCase));
                        break;
                    case TextSearchMode.EndsWith:
                        posts = posts.Where(x => x.Title.EndsWith(szTitle.Value));
                        break;
                    case TextSearchMode.EndsWithIgnoreCase:
                        posts =
                            posts.Where(
                                x => x.Title.EndsWith(szTitle.Value, StringComparison.CurrentCultureIgnoreCase));
                        break;
                    default:
                        posts = posts.Where(x => x.Title.ToLower().Contains(szTitle.Value.ToLower()));
                        break;
                }
            }

            // Body is specified.
            if (conditions.Body != null && !string.IsNullOrEmpty(conditions.Body.Value))
            {
                var szBody = conditions.Body;
                switch (szBody.Mode)
                {
                    case TextSearchMode.Contain:
                        posts = posts.Where(x => x.Body.Contains(szBody.Value));
                        break;
                    case TextSearchMode.Equal:
                        posts = posts.Where(x => x.Body.Equals(szBody.Value));
                        break;
                    case TextSearchMode.EqualIgnoreCase:
                        posts =
                            posts.Where(x => x.Body.Equals(szBody.Value, StringComparison.CurrentCultureIgnoreCase));
                        break;
                    case TextSearchMode.StartsWith:
                        posts = posts.Where(x => x.Body.StartsWith(szBody.Value));
                        break;
                    case TextSearchMode.StartsWithIgnoreCase:
                        posts =
                            posts.Where(
                                x => x.Body.StartsWith(szBody.Value, StringComparison.CurrentCultureIgnoreCase));
                        break;
                    case TextSearchMode.EndsWith:
                        posts = posts.Where(x => x.Body.EndsWith(szBody.Value));
                        break;
                    case TextSearchMode.EndsWithIgnoreCase:
                        posts =
                            posts.Where(
                                x => x.Body.EndsWith(szBody.Value, StringComparison.CurrentCultureIgnoreCase));
                        break;
                    default:
                        posts = posts.Where(x => x.Body.ToLower().Contains(szBody.Value.ToLower()));
                        break;
                }
            }

            // CreatedTime is specified.
            if (conditions.CreatedTime != null)
            {
                var created = conditions.CreatedTime;

                // From is defined.
                if (created.From != null)
                    posts = posts.Where(x => x.CreatedTime >= created.From.Value);

                // To is defined.
                if (created.To != null)
                    posts = posts.Where(x => x.CreatedTime <= created.To.Value);
            }

            // Last modified is specified.
            if (conditions.LastModifiedTime != null)
            {
                var lastModifiedTime = conditions.LastModifiedTime;

                // From is defined.
                if (lastModifiedTime.From != null)
                    posts = posts.Where(x => x.LastModifiedTime >= lastModifiedTime.From.Value);

                // To is defined.
                if (lastModifiedTime.To != null)
                    posts = posts.Where(x => x.LastModifiedTime <= lastModifiedTime.To.Value);
            }

            return posts;
        }

        #endregion
    }
}