using AuthenticationDb.Models.Entities;
using ServiceShared.Services;

namespace AuthenticationMicroService.Services
{
    public class ProfileCacheService : BaseKeyValueCacheService<int, User>
    {
    }
}