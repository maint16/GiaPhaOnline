using Newtonsoft.Json;

namespace AppDb.Models.Entities
{
    public class ActivationToken
    {
        #region Properties

        /// <summary>
        /// Activation token code.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Id of user.
        /// </summary>
        public int OwnerId { get; set; }

        /// <summary>
        /// Issued time.
        /// </summary>
        public double IssuedTime { get; set; }

        /// <summary>
        /// Expiration time.
        /// </summary>
        public double ExpiredTime { get; set; }

        #endregion

        #region Navigation properties
        
        /// <summary>
        /// One access token belongs to only one user.
        /// </summary>
        [JsonIgnore]
        public virtual User Owner { get; set; }

        #endregion

        #region Constructors

        public ActivationToken()
        {
        }

        public ActivationToken(string code, int ownerId, double issuedTime, double expiredTime)
        {
            Code = code;
            OwnerId = ownerId;
            IssuedTime = issuedTime;
            ExpiredTime = expiredTime;
        }

        #endregion
    }
}