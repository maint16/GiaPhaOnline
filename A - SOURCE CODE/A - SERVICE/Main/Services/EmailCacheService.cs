using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Main.Interfaces.Services;
using Main.Models;
using Microsoft.AspNetCore.Hosting;

namespace Main.Services
{
    /// <summary>
    /// Service for email caching.
    /// </summary>
    public class EmailCacheService : ValueCacheBaseService<string, EmailCacheOption>, IEmailCacheService
    {
        #region Properties

        /// <summary>
        /// Hosting environment.
        /// </summary>
        public IHostingEnvironment HostingEnvironment { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Read configuration from cache options.
        /// </summary>
        /// <param name="cacheOptions"></param>
        public void ReadConfiguration(IDictionary<string, EmailCacheOption> cacheOptions)
        {
            // Get through every template name.
            foreach (var szTemplateName in cacheOptions.Keys)
            {
                // Find caching option.
                var option = cacheOptions[szTemplateName];

                // Full file path.
                string fullPath;

                // Path url is absolute.
                if (option.IsAbsolutePath)
                    fullPath = option.FileName;
                else
                    fullPath = Path.Combine(HostingEnvironment.ContentRootPath, option.FileName);
                
                // File doesn't exist.
                if (string.IsNullOrWhiteSpace(fullPath) || !File.Exists(fullPath))
                    continue;

                // Read file content.
                var szFileContent = File.ReadAllText(fullPath);
                option.Content = szFileContent;

                Add(szTemplateName, option);
            }
        }

        /// <summary>
        /// Override find key method.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override string FindKey(string key)
        {
            return key.ToLower();
        }

        #endregion
    }
}