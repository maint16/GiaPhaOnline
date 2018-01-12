using System;
using System.Linq;
using SystemDatabase.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Enumerations;
using Shared.Interfaces.Repositories;
using Shared.ViewModels.SignalrConnections;

namespace Shared.Repositories
{
    public class RepositorySignalrConnection : ParentRepository<SignalrConnection>, IRepositorySignalrConnection
    {
        #region Constructors

        public RepositorySignalrConnection(DbContext dbContext) : base(dbContext)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Search records with specific conditions.
        /// </summary>
        /// <param name="connections"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public IQueryable<SignalrConnection> Search(IQueryable<SignalrConnection> connections,
            FindSignalrConnectionViewModel conditions)
        {
            // Conditions is invalid.
            if (conditions == null)
                return connections;

            if (conditions.Id != null && !string.IsNullOrEmpty(conditions.Id.Value))
            {
                var szIndex = conditions.Id;
                switch (szIndex.Mode)
                {
                    case TextSearchMode.Contain:
                        connections = connections.Where(x => x.Id.Contains(szIndex.Value));
                        break;
                    case TextSearchMode.Equal:
                        connections = connections.Where(x => x.Id.Equals(szIndex.Value));
                        break;
                    case TextSearchMode.EqualIgnoreCase:
                        connections =
                            connections.Where(x => x.Id.Equals(szIndex.Value, StringComparison.CurrentCultureIgnoreCase));
                        break;
                    case TextSearchMode.StartsWith:
                        connections = connections.Where(x => x.Id.StartsWith(szIndex.Value));
                        break;
                    case TextSearchMode.StartsWithIgnoreCase:
                        connections =
                            connections.Where(
                                x => x.Id.StartsWith(szIndex.Value, StringComparison.CurrentCultureIgnoreCase));
                        break;
                    case TextSearchMode.EndsWith:
                        connections = connections.Where(x => x.Id.EndsWith(szIndex.Value));
                        break;
                    case TextSearchMode.EndsWithIgnoreCase:
                        connections =
                            connections.Where(
                                x => x.Id.EndsWith(szIndex.Value, StringComparison.CurrentCultureIgnoreCase));
                        break;
                    default:
                        connections = connections.Where(x => x.Id.ToLower().Contains(szIndex.Value.ToLower()));
                        break;
                }
            }

            if (conditions.Owner != null)
                connections = connections.Where(x => x.OwnerId == conditions.Owner.Value);

            var timeSearchCreated = conditions.CreatedTime;
            if (timeSearchCreated != null)
            {
                var from = timeSearchCreated.From;
                var to = timeSearchCreated.To;
                if (from != null)
                    connections = connections.Where(x => x.CreatedTime >= from.Value);
                if (to != null)
                    connections = connections.Where(x => x.CreatedTime <= to.Value);
            }

            return connections;
        }

        #endregion
    }
}