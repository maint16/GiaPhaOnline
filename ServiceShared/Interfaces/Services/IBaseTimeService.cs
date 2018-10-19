using System;

namespace ServiceShared.Interfaces.Services
{
    public interface IBaseTimeService
    {
        #region Methods

        /// <summary>
        /// Convert time utc to unix utc time.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        double DateTimeUtcToUnix(DateTime dateTime);

        /// <summary>
        /// Convert utc unix time to utc datetime.
        /// </summary>
        /// <param name="unixTime"></param>
        /// <returns></returns>
        DateTime UnixToDateTimeUtc(double unixTime);

        #endregion
    }
}