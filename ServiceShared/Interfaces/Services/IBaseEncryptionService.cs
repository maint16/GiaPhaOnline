namespace ServiceShared.Interfaces.Services
{
    public interface IBaseEncryptionService
    {
        /// <summary>
        ///     Hash a string by using md5.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        string Md5Hash(string input);
    }
}