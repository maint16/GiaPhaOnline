using System.Linq;

namespace Shared.ViewModels
{
    public class SearchResult<T> where T : class 
    {
        /// <summary>
        /// List of searched records.
        /// </summary>
        public T Records { get; set; }

        /// <summary>
        /// Total records match the condition.
        /// </summary>
        public int Total { get; set; }
    }
}