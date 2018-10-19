using AuthenticationDb.Models.Entities;
using ServiceShared.Interfaces.Services;
using ServiceShared.Services;

namespace AuthenticationMicroService.Services
{
    public class ProfileCacheService : BaseKeyValueCacheService<int, User>
    {
    }
}