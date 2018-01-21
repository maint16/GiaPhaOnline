using SystemConstant.Enumerations;
using Newtonsoft.Json;

namespace Shared.ViewModels.Accounts
{
    public class AccountViewModel
    {
        /// <summary>
        ///     Id of account.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? Id { get; set; }

        /// <summary>
        ///     Email of account.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Email { get; set; }

        /// <summary>
        ///     Nickname of account.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Nickname { get; set; }

        /// <summary>
        ///     Account password.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Password { get; set; }

        /// <summary>
        ///     Account status.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public AccountStatus? Status { get; set; }

        /// <summary>
        ///     When the account was created.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public double? JoinedTime { get; set; }

        /// <summary>
        ///     When the account information was lastly changed.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public double? LastModifiedTime { get; set; }
    }
}