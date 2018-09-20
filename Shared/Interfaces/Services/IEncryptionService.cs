namespace Shared.Interfaces.Services
{
    public interface IEncryptionService
    {
        /// <summary>
        ///     Hash a string by using md5.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        string Md5Hash(string input);
    }
}