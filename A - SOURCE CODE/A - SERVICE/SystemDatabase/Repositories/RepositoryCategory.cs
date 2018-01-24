using System;
using System.Linq;
using SystemConstant.Enumerations;
using SystemDatabase.Interfaces.Repositories;
using SystemDatabase.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace SystemDatabase.Repositories
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

        

        #endregion
    }
}