using AppModel.Enumerations;
using Microsoft.AspNetCore.Authorization;

namespace Main.Authentications.Requirements
{
    public class RoleRequirement : IAuthorizationRequirement
    {
        #region Constructor

        /// <summary>
        ///     Initiate requirement with roles.
        /// </summary>
        /// <param name="roles"></param>
        public RoleRequirement(AccountRole[] roles)
        {
            _roles = roles;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Client valid roles.
        /// </summary>
        private readonly AccountRole[] _roles;

        /// <summary>
        ///     List of accessible role.
        /// </summary>
        public AccountRole[] Roles => _roles;

        #endregion
    }
}