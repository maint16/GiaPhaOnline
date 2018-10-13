using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AppModel.Models
{
    public class AppJwtModel
    {
        #region Properties

        /// <summary>
        ///     Issuer of jwt.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        ///     The sub (subject) claim identifies the principal that is the subject of the JWT. The claims in a JWT are normally
        ///     statements about the subject. The subject value MUST either be scoped to be locally unique in the context of the
        ///     issuer or be globally unique. The processing of this claim is generally application specific. The sub value is a
        ///     case-sensitive string containing a StringOrURI value.
        ///     Use of this claim is OPTIONAL
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        ///     The aud (audience) claim identifies the recipients that the JWT is intended for.
        ///     Each principal intended to process the JWT MUST identify itself with a value in the audience claim. If the
        ///     principal processing the claim does not identify itself with a value in the aud claim when this claim is present,
        ///     then the JWT MUST be rejected.
        ///     In the general case, the aud value is an array of case-sensitive strings, each containing a StringOrURI value. In
        ///     the special case when the JWT has one audience, the aud value MAY be a single case-sensitive string containing a
        ///     StringOrURI value. The interpretation of audience values is generally application specific. Use of this claim is
        ///     OPTIONAL.
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        ///     he exp (expiration time) claim identifies the expiration time on or after which the JWT MUST NOT be accepted for
        ///     processing. The processing of the exp claim requires that the current date/time MUST be before the expiration
        ///     date/time listed in the exp claim. Implementers MAY provide for some small leeway, usually no more than a few
        ///     minutes, to account for clock skew.
        ///     Its value MUST be a number containing a NumericDate value.
        ///     Use of this claim is OPTIONAL.
        /// </summary>
        public int LifeTime { get; set; }

        /// <summary>
        ///     DeviceId which is used for signing to token.
        /// </summary>
        public string SecurityKey { get; set; }

        /// <summary>
        ///     Credential which is used for signing token.
        /// </summary>
        private SigningCredentials _signingCredentials;

        /// <summary>
        ///     DeviceId which is used for calculating credential.
        /// </summary>
        private SymmetricSecurityKey _symmetricSecurityKey;

        /// <summary>
        ///     DeviceId which is used for signing on token.
        /// </summary>
        public SymmetricSecurityKey SigningKey
        {
            get
            {
                if (_symmetricSecurityKey == null)
                    _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecurityKey));

                return _symmetricSecurityKey;
            }
        }

        /// <summary>
        ///     Credential which is used for signing on token.
        /// </summary>
        public SigningCredentials SigningCredentials
        {
            get
            {
                if (_signingCredentials == null)
                    _signingCredentials = new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256);

                return _signingCredentials;
            }
        }

        #endregion
    }
}