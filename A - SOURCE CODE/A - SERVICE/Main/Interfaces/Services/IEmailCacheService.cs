using Main.Models;

namespace Main.Interfaces.Services
{
    public interface IEmailCacheService : IValueCacheService<string, EmailCacheOption>
    {
    }
}