using AuthenticationDb.Models.Entities;

namespace AuthenticationMicroService.Services
{
    public class ProfileCacheService : ValueCacheBaseService<int, User>
    {
    }
}
