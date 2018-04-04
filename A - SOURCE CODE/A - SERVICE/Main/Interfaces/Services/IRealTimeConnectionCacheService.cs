using SystemDatabase.Models.Entities;

namespace Main.Interfaces.Services
{
    /// <summary>
    /// Service which is for caching connection id with user instances.
    /// </summary>
    public interface IRealTimeConnectionCacheService : IValueCacheService<string, Account>
    {
        
    }
}