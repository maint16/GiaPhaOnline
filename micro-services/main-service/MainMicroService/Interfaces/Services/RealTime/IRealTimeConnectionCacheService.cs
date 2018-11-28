using MainDb.Models.Entities;
using ServiceShared.Interfaces.Services;

namespace MainMicroService.Interfaces.Services.RealTime
{
    /// <summary>
    ///     Service which is for caching connection id with user instances.
    /// </summary>
    public interface IRealTimeConnectionCacheService : IBaseKeyValueCacheService<string, User>
    {
    }
}