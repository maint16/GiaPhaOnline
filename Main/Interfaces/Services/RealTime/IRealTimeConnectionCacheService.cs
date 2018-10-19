using AppBusiness.Interfaces;
using AppDb.Models.Entities;
using ServiceShared.Interfaces.Services;

namespace Main.Interfaces.Services.RealTime
{
    /// <summary>
    ///     Service which is for caching connection id with user instances.
    /// </summary>
    public interface IRealTimeConnectionCacheService : IValueCacheService<string, User>
    {
    }
}