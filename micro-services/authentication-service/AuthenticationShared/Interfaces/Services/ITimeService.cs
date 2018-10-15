using System;

namespace AuthenticationShared.Interfaces.Services
{
    public interface ITimeService
    {
        double DateTimeUtcToUnix(DateTime dateTime);

        DateTime UnixToDateTimeUtc(double unixTime);
    }
}
