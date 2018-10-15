using Microsoft.AspNetCore.Authorization;
using Shared.Enumerations;

namespace Main.Authentications.Requirements
{
    public class RoleRequirement : IAuthorizationRequirement
    {
        #region Constructor

        /// <summary>
        ///     Initiate requirement with roles.
        /// </summary>
        /// <param name="roles"></param>
        public RoleRequirement(UserRole[] roles)
        {
            _roles = roles;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Client valid roles.
        /// </summary>
        private readonly UserRole[] _roles;

        /// <summary>
        ///     List of accessible role.
        /// </summary>
        public UserRole[] Roles => _roles;

        #endregion
    }
}