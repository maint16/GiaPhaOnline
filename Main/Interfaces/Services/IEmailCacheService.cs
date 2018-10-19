using Main.Models;
using ServiceShared.Interfaces.Services;

namespace Main.Interfaces.Services
{
    public interface IEmailCacheService : IBaseKeyValueCacheService<string, EmailCacheOption>
    {
    }
}