using Newtonsoft.Json;

namespace Main.Models
{
    public class EmailCacheOption
    {
        #region Properties

        /// <summary>
        /// Name of file.
        /// </summary>
        public string FileName { get; set; }
        
        /// <summary>
        /// Whether path is absolute or not.
        /// </summary>
        public bool IsAbsolutePath { get; set; }

        /// <summary>
        /// Email subject.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Content of cache.
        /// </summary>
        [JsonIgnore]
        public string Content { get; set; }

        /// <summary>
        /// Whether content is html or not.
        /// </summary>
        public bool IsHtmlContent { get; set; }

        #endregion
    }
}