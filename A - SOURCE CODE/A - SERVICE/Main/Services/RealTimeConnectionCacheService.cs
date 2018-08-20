using AppDb.Models.Entities;
using Main.Interfaces.Services;

namespace Main.Services
{
    public class RealTimeConnectionCacheService : ValueCacheBaseService<string, Account>, IRealTimeConnectionCacheService
    {
        #region Methods

        /// <summary>
        /// All keys should be lower cased.
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