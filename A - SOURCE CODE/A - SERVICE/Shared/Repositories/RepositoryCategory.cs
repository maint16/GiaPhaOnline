using System;
using System.Linq;
using SystemDatabase.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Enumerations;
using Shared.Interfaces.Repositories;
using Shared.ViewModels.Categories;

namespace Shared.Repositories
{
    public class RepositoryCategory : ParentRepository<Category>, IRepositoryCategory
    {
        #region Constructor

        /// <summary>
        ///     Initiate repository with database context.
        /// </summary>
        /// <param name="dbContext"></param>
        public RepositoryCategory(
            DbContext dbContext) : base(dbContext)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Search categories by using specific conditions.
        /// </summary>
        /// <param name="categories"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public IQueryable<Category> Search(IQueryable<Category> categories, SearchCategoryViewModel conditions)
        {
            // Id has been defined.
            if (conditions.Id != null)
                categories = categories.Where(x => x.Id == conditions.Id.Value);

            // Creator has been defined.
            if (conditions.CreatorId != null)
                categories = categories.Where(x => x.CreatorId == conditions.CreatorId.Value);

            // Name search condition has been defined.
            if (conditions.Name != null && !string.IsNullOrWhiteSpace(conditions.Name.Value))
            {
                var szName = conditions.Name;
                switch (szName.Mode)
                {
                    case TextSearchMode.Contain:
                        categories = categories.Where(x => x.Name.Contains(szName.Value));
                        break;
                    case TextSearchMode.Equal:
                        categories = categories.Where(x => x.Name.Equals(szName.Value));
                        break;
                    case TextSearchMode.EqualIgnoreCase:
                        categories =
                            categories.Where(x => x.Name.Equals(szName.Value, StringComparison.CurrentCultureIgnoreCase));
                        break;
                    case TextSearchMode.StartsWith:
                        categories = categories.Where(x => x.Name.StartsWith(szName.Value));
                        break;
                    case TextSearchMode.StartsWithIgnoreCase:
                        categories =
                            categories.Where(
                                x => x.Name.StartsWith(szName.Value, StringComparison.CurrentCultureIgnoreCase));
                        break;
                    case TextSearchMode.EndsWith:
                        categories = categories.Where(x => x.Name.EndsWith(szName.Value));
                        break;
                    case TextSearchMode.EndsWithIgnoreCase:
                        categories =
                            categories.Where(
                                x => x.Name.EndsWith(szName.Value, StringComparison.CurrentCultureIgnoreCase));
                        break;
                    default:
                        categories = categories.Where(x => x.Name.ToLower().Contains(szName.Value.ToLower()));
                        break;
                }
            }

            // CreatedTime time range has been defined.
            if (conditions.CreatedTime != null)
            {
                // Start time is defined.
                if (conditions.CreatedTime.From != null)
                    categories = categories.Where(x => x.CreatedTime >= conditions.CreatedTime.From.Value);

                // End time is defined.
                if (conditions.CreatedTime.To != null)
                    categories = categories.Where(x => x.CreatedTime <= conditions.CreatedTime.To.Value);
            }

            // Last modified time range has been defined.
            if (conditions.LastModifiedTime != null)
            {
                // Start time is defined.
                if (conditions.LastModifiedTime.From != null)
                    categories = categories.Where(x => x.LastModifiedTime >= conditions.LastModifiedTime.From.Value);

                // End time is defined.
                if (conditions.LastModifiedTime.To != null)
                    categories = categories.Where(x => x.LastModifiedTime <= conditions.LastModifiedTime.To.Value);
            }

            return categories;
        }

        #endregion
    }
}