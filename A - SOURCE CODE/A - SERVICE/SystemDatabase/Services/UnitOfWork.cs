using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SystemConstant.Enumerations;
using SystemConstant.Enumerations.Order;
using SystemConstant.Models;
using SystemDatabase.Interfaces;
using SystemDatabase.Interfaces.Repositories;
using SystemDatabase.Repositories;
using Microsoft.EntityFrameworkCore;

namespace SystemDatabase.Services
{
    public class UnitOfWork<T> : IUnitOfWork, IDisposable where T : DbContext
    {
        #region Constructors

        /// <summary>
        ///     Initiate unit of work with database context provided by Entity Framework.
        /// </summary>
        /// <param name="dbContext"></param>
        public UnitOfWork(T dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Variables

        /// <summary>
        ///     Whether the instance has been disposed or not.
        /// </summary>
        private bool _disposed;

        /// <summary>
        ///     Provide methods to access confession database.
        /// </summary>
        private readonly DbContext _dbContext;

        #endregion

        #region Properties

        /// <summary>
        ///     Provide access to accounts database.
        /// </summary>
        private IRepositoryAccount _repositoryAccounts;

        /// <summary>
        ///     Provide access to categories database.
        /// </summary>
        private IRepositoryCategory _repositoryCategories;

        /// <summary>
        /// Provide access to FollowCategory table.
        /// </summary>
        private IRepositoryFollowCategory _repositoryFollowCategory;

        /// <summary>
        /// Provide access to categorization database.
        /// </summary>
        private IRepositoryCategorization _repositoryCategorization;

        /// <summary>
        ///     Provide access to comments database.
        /// </summary>
        private IRepositoryComment _repositoryComment;

        /// <summary>
        ///     Provide functions to access comment reports database.
        /// </summary>
        private IRepositoryCommentReport _repositoryCommentReport;

        /// <summary>
        ///     Provide access to post report database.
        /// </summary>
        private IRepositoryPostReport _repositoryPostReport;

        /// <summary>
        ///     Provides access to post database.
        /// </summary>
        private IRepositoryPost _repositoryPost;

        /// <summary>
        ///     Provide access to token database.
        /// </summary>
        private IRepositoryToken _repositoryToken;

        /// <summary>
        ///     Provide access to signalr connection database.
        /// </summary>
        private IRepositorySignalrConnection _repositorySignalrConnection;

        /// <summary>
        ///     Provides functions to access account database.
        /// </summary>
        public IRepositoryAccount RepositoryAccounts => _repositoryAccounts ?? (_repositoryAccounts = new RepositoryAccount(_dbContext));

        /// <summary>
        ///     Provides functions to access categories database.
        /// </summary>
        public IRepositoryCategory RepositoryCategories => _repositoryCategories ?? (_repositoryCategories = new RepositoryCategory(_dbContext));

        /// <summary>
        /// Provides function to access follow category table.
        /// </summary>
        public IRepositoryFollowCategory RepositoryFollowCategory =>
            _repositoryFollowCategory ?? (_repositoryFollowCategory = new RepositoryFollowCategory(_dbContext));

        /// <summary>
        /// Provides functions to access categorizations database.
        /// </summary>
        public IRepositoryCategorization RepositoryCategorizations =>
            _repositoryCategorization ?? (_repositoryCategorization = new RepositoryCategorization(_dbContext));

        /// <summary>
        ///     Provides functions to access comments database.
        /// </summary>
        public IRepositoryComment RepositoryComments => _repositoryComment ?? (_repositoryComment = new RepositoryComment(_dbContext));


        /// <summary>
        ///     Provides functions to access to post reports database.
        /// </summary>
        public IRepositoryPostReport RepositoryPostReports
            => _repositoryPostReport ?? (_repositoryPostReport = new RepositoryPostReport(_dbContext));

        /// <summary>
        ///     Provides functions to access post database.
        /// </summary>
        public IRepositoryPost RepositoryPosts => _repositoryPost ?? (_repositoryPost = new RepositoryPost(_dbContext));

        /// <summary>
        ///     Provides functions to access realtime connection database.
        /// </summary>
        public IRepositorySignalrConnection RepositorySignalrConnections => _repositorySignalrConnection ??
                                                                            (_repositorySignalrConnection = new RepositorySignalrConnection(_dbContext));

        /// <summary>
        ///     Provides functions to access comment reports database.
        /// </summary>
        public IRepositoryCommentReport RepositoryCommentReports => _repositoryCommentReport ??
                                                                    (_repositoryCommentReport =
                                                                        new RepositoryCommentReport(_dbContext))
        ;

        /// <summary>
        ///     Provides function to access token database.
        /// </summary>
        public IRepositoryToken RepositoryTokens => _repositoryToken ?? (_repositoryToken = new RepositoryToken(_dbContext));

        #endregion

        #region Methods

        /// <summary>
        ///     Do pagination on a specific list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public IQueryable<T> Paginate<T>(IQueryable<T> list, Pagination pagination)
        {
            if (pagination == null)
                return list;
            return list.Skip(pagination.Index * pagination.Records).Take(pagination.Records);
        }

        /// <summary>
        ///     Sort a list by using specific property enumeration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="sortDirection"></param>
        /// <param name="sortProperty"></param>
        /// <returns></returns>
        public IQueryable<T> Sort<T>(IQueryable<T> list, SortDirection sortDirection, Enum sortProperty)
        {
            string sortMethod;
            if (sortDirection == SortDirection.Ascending)
                sortMethod = "OrderBy";
            else
                sortMethod = "OrderByDescending";

            // Search parameter expression.
            var parameterExpression = Expression.Parameter(list.ElementType, "p");

            // Search name of property which should be used for sorting.
            var sortPropertyName = Enum.GetName(sortProperty.GetType(), sortProperty);
            if (string.IsNullOrEmpty(sortPropertyName))
                return list;

            // Search member expression.
            var memberExpression = Expression.Property(parameterExpression, sortPropertyName);

            var lamdaExpression = Expression.Lambda(memberExpression, parameterExpression);

            var methodCallExpression = Expression.Call(
                typeof(Queryable),
                sortMethod,
                new[] { list.ElementType, memberExpression.Type },
                list.Expression,
                Expression.Quote(lamdaExpression));

            return list.Provider.CreateQuery<T>(methodCallExpression);
        }

        /// <summary>
        ///     Search property base on searching mode.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="records"></param>
        /// <param name="property"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public IQueryable<T> SearchPropertyText<T>(IQueryable<T> records, Func<T, string> property, TextSearch search)
        {
            if (search == null || string.IsNullOrWhiteSpace(search.Value))
                return records;

            switch (search.Mode)
            {
                case TextSearchMode.Contain:
                    records = records.Where(x => property(x).Contains(search.Value));
                    break;
                case TextSearchMode.Equal:
                    records = records.Where(x => property(x).Equals(search.Value));
                    break;
                case TextSearchMode.EqualIgnoreCase:
                    records =
                        records.Where(x => property(x).Equals(search.Value, StringComparison.CurrentCultureIgnoreCase));
                    break;
                case TextSearchMode.StartsWith:
                    records = records.Where(x => property(x).StartsWith(search.Value));
                    break;
                case TextSearchMode.StartsWithIgnoreCase:
                    records =
                        records.Where(
                            x => property(x).StartsWith(search.Value, StringComparison.CurrentCultureIgnoreCase));
                    break;
                case TextSearchMode.EndsWith:
                    records = records.Where(x => property(x).EndsWith(search.Value));
                    break;
                case TextSearchMode.EndsWithIgnoreCase:
                    records =
                        records.Where(
                            x => property(x).EndsWith(search.Value, StringComparison.CurrentCultureIgnoreCase));
                    break;
                default:
                    records = records.Where(x => property(x).ToLower().Contains(search.Value.ToLower()));
                    break;
            }
            return records;
        }

        /// <summary>
        ///     Save changes into database.
        /// </summary>
        /// <returns></returns>
        public int Commit()
        {
            return _dbContext.SaveChanges();
        }

        /// <summary>
        ///     Save changes into database asynchronously.
        /// </summary>
        /// <returns></returns>
        public async Task<int> CommitAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        ///     Dispose the instance and free it from memory.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            // Object has been disposed.
            if (_disposed)
                return;

            // Object is being disposed.
            if (disposing)
                _dbContext.Dispose();

            _disposed = true;
        }

        /// <summary>
        ///     Dispose the instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}