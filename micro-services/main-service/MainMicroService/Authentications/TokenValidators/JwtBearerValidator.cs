using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace MainMicroService.Authentications.TokenValidators
{
    public class JwtBearerValidator : ISecurityTokenValidator
    {
        #region Methods

        /// <summary>
        ///     Whether validator can read token or not.
        /// </summary>
        /// <param name="securityToken"></param>
        /// <returns></returns>
        public bool CanReadToken(string securityToken)
        {
            return true;
        }

        /// <summary>
        ///     Callback which is called when token is being validated.
        ///     Claims will be generated in this function.
        /// </summary>
        /// <param name="securityToken"></param>
        /// <param name="validationParameters"></param>
        /// <param name="validatedToken"></param>
        /// <returns></returns>
        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters,
            out SecurityToken validatedToken)
        {
            // Handler which is for handling security token.
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            // Validate jwt.
            var claimsPrincipal =
                jwtSecurityTokenHandler.ValidateToken(securityToken, validationParameters, out validatedToken);
            return claimsPrincipal;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Whether this validator can validate token or not.
        /// </summary>
        public bool CanValidateToken => true;

        /// <summary>
        ///     Maximum token size.
        /// </summary>
        public int MaximumTokenSizeInBytes { get; set; }

        #endregion
    }
}