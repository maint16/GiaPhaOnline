﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AppDb.Models.Entities;
using Main.ViewModels.Category;
using Main.ViewModels.CategoryGroup;
using Shared.Models;

namespace Main.Interfaces.Services.Businesses
{
    public interface ICategoryGroupService
    {
        #region Methods

        /// <summary>
        /// Add category group asynchronously.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<CategoryGroup> AddCategoryGroup(AddCategoryGroupViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Edit category group asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<CategoryGroup> EditCategoryGroup(int id, EditCategoryGroupViewModel model,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Search for category groups using specific conditions.
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SearchResult<IList<CategoryGroup>>> SearchCategoryGroupsAsync(SearchCategoryGroupViewModel conditions, CancellationToken cancellationToken = default(CancellationToken));

        #endregion
    }
}