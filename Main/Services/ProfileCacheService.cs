using AppDb.Models.Entities;
using ServiceShared.Services;

namespace Main.Services
{
    public class ProfileCacheService : BaseKeyValueCacheService<int, User>
    {
    }
}