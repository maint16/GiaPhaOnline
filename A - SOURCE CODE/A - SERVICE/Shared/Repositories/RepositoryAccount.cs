using System.Linq;
using SystemDatabase.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Repositories;
using Shared.ViewModels.Accounts;

namespace Shared.Repositories
{
    public class RepositoryAccount : ParentRepository<Account>, IRepositoryAccount
    {
        #region Constructors

        /// <summary>
        ///     Initiate repository with dependency injection.
        /// </summary>
        /// <param name="dbContext"></param>
        public RepositoryAccount(
            DbContext dbContext) : base(dbContext)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Search accounts using specific conditions.
        /// </summary>
        /// <param name="accounts"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public IQueryable<Account> Search(IQueryable<Account> accounts, SearchAccountViewModel conditions)
        {
            // Id has been identified.
            if (conditions.Id != null)
                accounts = accounts.Where(x => x.Id == conditions.Id.Value);

            // Email & nickname are defined.
            accounts = SearchPropertyText(accounts, x => x.Email, conditions.Email);
            accounts = SearchPropertyText(accounts, x => x.Nickname, conditions.Nickname);

            // Statuses have been defined.
            if (conditions.Statuses != null)
            {
                // Construct the statuses as a list.
                var statuses = conditions.Statuses.ToList();
                accounts = accounts.Where(x => statuses.Contains(x.Status));
            }

            // JoinedTime has been defined.
            //accounts = SearchPropertyNumeric(accounts, x => x.JoinedTime, conditions.JoinedTime);
            //accounts = SearchPropertyNumeric(accounts, x => x.LastModifiedTime, conditions.LastModifiedTime);
            
            return accounts;
        }

        #endregion
    }
}