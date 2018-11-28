using MainMicroService.Models;
using ServiceShared.Interfaces.Services;

namespace MainMicroService.Interfaces.Services
{
    public interface IEmailCacheService : IBaseKeyValueCacheService<string, EmailCacheOption>
    {
    }
}