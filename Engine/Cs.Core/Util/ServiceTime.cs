namespace Cs.Core.Util
{
    using System;
    using Cs.Core.Core;
    using Cs.Logging;

    public static class ServiceTime
    {
        private const int RecentTimeResolutionExp = 5;
        private static AtomicFlag recentUpdateFlag = new AtomicFlag(initialValue: false);
        private static volatile int lastRecentTicks = -1;
        private static DateTime lastRecentDateTime = DateTime.MinValue;

        public static DateTime Now => DateTime.UtcNow + UtcOffset + DebugOffset;
        public static DateTime UtcNow => DateTime.UtcNow + DebugOffset;
        public static DateTime Epoch => TimeHelper.UtcEpoch + UtcOffset;
        public static TimeSpan UtcOffset { get; private set; } = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
        public static TimeSpan DebugOffset { get; private set; } = TimeSpan.Zero;
        public static TimeZoneInfo TimezoneInfo { get; private set; } = TimeZoneInfo.Local;
        public static DateTime Recent => GetRecentTime();
        public static DateTime Forever { get; } = DateTime.Parse("9000-1-1");
        public static string NowDefaultString => Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        public static string NowFileString => Now.ToString("yyyy.MM.dd-HH.mm.ss.fff");
        public static string RecentDefaultString => Recent.ToString("yyyy-MM-dd HH:mm:ss.fff");
        public static string RecentFileString => Recent.ToString("yyyy.MM.dd-HH.mm.ss.fff");
        public static DateTime OriginalNow => DateTime.UtcNow + UtcOffset;

        public static void Initialize(string timezoneId)
        {
            TimezoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezoneId)
                ?? throw new Exception($"[TimeZone] get systemTimeZone failed. timeZoneId:{timezoneId}");
            UtcOffset = TimezoneInfo.BaseUtcOffset;

            Log.Info($"[ServiceTime] initialized to {TimezoneInfo}. id:{timezoneId} offset:{UtcOffset}");

            var debugTimeString = Environment.GetEnvironmentVariable("CS_TIME");
            //TryParse는 실패할 경우 DateTime.MinValue을 리턴합니다.
            _ = DateTime.TryParse(debugTimeString, out DateTime debugTime);

            if (debugTime > DateTime.MinValue)
            {
                Log.DebugBold($"[ServiceTime] CS_TIME: {debugTime}");
                SetTimeTo(debugTime);
            }
        }

        public static void InitializeWithSummerTime(string timezoneId, DateTime currentUtc)
        {
            TimezoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezoneId)
                ?? throw new Exception($"[TimeZone] get systemTimeZone failed. timeZoneId:{timezoneId}");

            UtcOffset = TimezoneInfo.GetUtcOffset(currentUtc);

            bool isSummerTime = TimezoneInfo.IsAmbiguousTime(currentUtc) ||
                TimezoneInfo.IsDaylightSavingTime(currentUtc);

            Log.Info($"[ServiceTime] initialized to {TimezoneInfo}. id:{timezoneId} offset:{UtcOffset} isSummerTime:{isSummerTime}");
        }

        public static DateTime FromUtcTime(DateTime utcTime) => utcTime + UtcOffset;
        public static DateTime FromUtcTime(long ticks) => new DateTime(ticks, DateTimeKind.Utc) + UtcOffset;
        public static DateTime FromLocalTime(DateTime localTime)
        {
            TimeSpan localOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
            return localTime - localOffset + UtcOffset;
        }

        public static DateTime ToUtcTime(DateTime serviceTime) => serviceTime - UtcOffset;

        public static double ToUnixTimeStamp(DateTime serviceTime) => (serviceTime - Epoch).TotalMilliseconds;

        public static bool IsBetween(DateTime start, DateTime end)
        {
            return ServiceTime.Now.IsBetween(start, end);
        }

        public static void SetTimeTo(DateTime time)
        {
            // 입력받은 시각이 최종 시각이 되도록 설정.
            DebugOffset = time - OriginalNow;
        }

        public static void SetDebugOffset(TimeSpan span)
        {
            // bridge -> gameserver로 동기화할 땐 DebugOffset을 직접 설정
            DebugOffset = span;
            lastRecentTicks = 0;
        }

        public static void ResetDebugOffset()
        {
            DebugOffset = TimeSpan.Zero;
        }

        private static DateTime GetRecentTime()
        {
            // https://stackoverflow.com/questions/4075525/why-are-datetime-now-datetime-utcnow-so-slow-expensive
            int tickCount = Environment.TickCount >> RecentTimeResolutionExp;
            while (tickCount != lastRecentTicks)
            {
                if (recentUpdateFlag.On())
                {
                    lastRecentTicks = tickCount;
                    lastRecentDateTime = ServiceTime.Now;
                    recentUpdateFlag.Off();
                    break;
                }

                tickCount = Environment.TickCount >> RecentTimeResolutionExp;
            }

            return lastRecentDateTime;
        }
    }
}