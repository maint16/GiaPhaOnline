using System;

namespace Shared.Interfaces.Services
{
    public interface ITimeService
    {
        double DateTimeUtcToUnix(DateTime dateTime);

        DateTime UnixToDateTimeUtc(double unixTime);
    }
}