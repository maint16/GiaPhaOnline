using Newtonsoft.Json;
using Shared.ViewModels.Accounts;

namespace Shared.ViewModels.Categories
{
    public class CategoryViewModel
    {
        /// <summary>
        ///     Id of category
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? Id { get; set; }

        /// <summary>
        ///     Person who created the categories.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public AccountViewModel Creator { get; set; }

        /// <summary>
        ///     Name of category.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        ///     When the category created.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public double? Created { get; set; }

        /// <summary>
        ///     When the category was lastly modified.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public double? LastModified { get; set; }
    }
}