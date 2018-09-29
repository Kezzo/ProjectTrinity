using System;

public static class UtcTimestampHelper 
{
    private static readonly DateTime UtcStartDateTime = new DateTime(1970, 1, 1);


    public static Int64 GetCurrentUtcTimestamp()
    {
        return DateTime.UtcNow.Subtract(UtcStartDateTime).Seconds;
    }

    public static Int64 GetCurrentUtcMsTimestamp()
    {
        return DateTime.UtcNow.Subtract(UtcStartDateTime).Milliseconds;
    }
}
