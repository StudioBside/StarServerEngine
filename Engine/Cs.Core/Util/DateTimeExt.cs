namespace Cs.Core.Util
{
    using System;
    using System.Globalization;

    public static class DateTimeExt
    {
        public static bool IsBetween(this DateTime self, DateTime start, DateTime end)
        {
            return start <= self && self < end;
        }

        public static bool EqualsLite(this DateTime self, DateTime other)
        {
            return self.Year == other.Year &&
                   self.Month == other.Month &&
                   self.Day == other.Day &&
                   self.Hour == other.Hour &&
                   self.Minute == other.Minute &&
                   self.Second == other.Second;
        }

        public static string ToDefaultString(this DateTime self)
        {
            return self.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        public static string ToDateString(this DateTime self)
        {
            return self.ToString("yyyy-MM-dd");
        }

        public static string ToTimeString(this DateTime self)
        {
            return self.ToString("HH:mm:ss.fff");
        }

        public static string ToDateString(this DateTime self, CultureInfo cultures)
        {
            return self.ToString("yyyy-MM-dd ddd", cultures);
        }

        public static string ToFileString(this DateTime self)
        {
            return self.ToString("yyyy.MM.dd-HH.mm.ss.fff");
        }

        /// <summary>
        /// 해당 연도의 첫 번째 월요일이 포함된 주부터 yyyy01로 표현되는 아이디.
        /// 월요일 자정이 넘어갈 때마다 값이 바뀝니다.
        /// </summary>
        public static WeeklyId GetWeeklyId(this DateTime self)
        {
            int weekOfYear = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                self,
                CalendarWeekRule.FirstFullWeek,
                DayOfWeek.Monday);

            // 첫 번째 월요일이 포함된 주부터 weekOfYear가 시작한다. 1월인 경우 예외처리가 필요하다.
            if (self.Year > 1 && self.Month == 1 && weekOfYear > 10)
            {
                // 1월인데 10보다 큰 주 수는 나올 수 없으므로. 연도를 하나 줄인다.
                return new WeeklyId(self.Year - 1, weekOfYear);
            }

            return new WeeklyId(self.Year, weekOfYear);
        }

        public static WeeklyId GetWeeklyId(this DateTime self, int weekDelta)
        {
            var timeSpan = TimeSpan.FromDays(7 * weekDelta);
            return (self + timeSpan).GetWeeklyId();
        }

        public static int GetDateId(this DateTime self)
        {
            return (self.Year * 10000) + (self.Month * 100) + self.Day;
        }
    }
}
