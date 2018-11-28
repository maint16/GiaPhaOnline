using MainDb.Models.Entities;
using MainMicroService.Interfaces.Services.RealTime;
using ServiceShared.Services;

namespace MainMicroService.Services
{
    public class RealTimeConnectionCacheService : BaseKeyValueCacheService<string, User>,
        IRealTimeConnectionCacheService
    {
        #region Methods

        /// <summary>
        ///     All keys should be lower cased.
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