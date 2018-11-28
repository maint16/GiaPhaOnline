using MainDb.Models.Entities;
using ServiceShared.Services;

namespace MainMicroService.Services
{
    public class ProfileCacheService : BaseKeyValueCacheService<int, User>
    {
    }
}