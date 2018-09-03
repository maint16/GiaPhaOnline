namespace Main.Interfaces.Services
{
    public interface IMustacheService
    {
        #region Methods

        /// <summary>
        /// Compile template with data to a string.
        /// </summary>
        /// <param name="szTemplate"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        string Compile(string szTemplate, object data);

        #endregion
    }
}