namespace Cs.Core.Util
{
    using System;

    public static class TimeHelper
    {
        private static DateTime utcEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static DateTime localEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);

        public static DateTime UtcEpoch => utcEpoch;
        public static DateTime LocalEpoch => localEpoch;

        public static DateTime UnixTimeToDateTime(long time, DateTimeKind kind)
        {
            if (kind == DateTimeKind.Utc)
            {
                return utcEpoch.AddSeconds(time);
            }

            // DateTimeKind.Unspecified 는 DateTimeKind.Local로 간주한다
            return localEpoch.AddSeconds(time);
        }

        // unix timestamp
        public static long ToUnixTime(this DateTime datetime)
        {
            if (datetime.Kind == DateTimeKind.Utc)
            {
                return (long)(datetime - utcEpoch).TotalSeconds;
            }

            // DateTimeKind.Unspecified 는 DateTimeKind.Local로 간주한다
            return (long)(datetime - localEpoch).TotalSeconds;
        }
    }
}
