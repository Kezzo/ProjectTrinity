using System;

namespace ProjectTrinity.Helper
{
    public static class UtcTimestampHelper
    {
        private static readonly DateTime UtcStartDateTime = new DateTime(1970, 1, 1);

        public static Int64 GetCurrentUtcTimestamp()
        {
            return (long) DateTime.UtcNow.Subtract(UtcStartDateTime).TotalSeconds;
        }

        public static Int64 GetCurrentUtcMsTimestamp()
        {
            return (long) DateTime.UtcNow.Subtract(UtcStartDateTime).TotalMilliseconds;
        }
    }
}