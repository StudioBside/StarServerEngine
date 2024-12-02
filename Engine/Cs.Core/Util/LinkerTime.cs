namespace Cs.Core.Util
{
    using System;
    using System.IO;
    using System.Reflection;

    public static class LinkerTime
    {
        public static DateTime GetLinkerTime(this Assembly assembly, TimeZoneInfo target)
        {
            var filePath = assembly.Location;
            const int PeHeaderOffset = 60;
            const int LinkerTimestampOffset = 8;

            var buffer = new byte[2048];

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var readbytes = stream.Read(buffer, 0, 2048);
            }

            var offset = BitConverter.ToInt32(buffer, PeHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(buffer, offset + LinkerTimestampOffset);

            var linkTimeUtc = TimeHelper.UnixTimeToDateTime(secondsSince1970, DateTimeKind.Utc);

            var tz = target ?? TimeZoneInfo.Local;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, tz);

            return localTime;
        }

        public static DateTime GetLinkerTime(string filePath, TimeZoneInfo target)
        {
            const int PeHeaderOffset = 60;
            const int LinkerTimestampOffset = 8;

            var buffer = new byte[2048];

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var readbytes = stream.Read(buffer, 0, 2048);
            }

            var offset = BitConverter.ToInt32(buffer, PeHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(buffer, offset + LinkerTimestampOffset);

            var linkTimeUtc = TimeHelper.UnixTimeToDateTime(secondsSince1970, DateTimeKind.Utc);

            var tz = target ?? TimeZoneInfo.Local;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, tz);

            return localTime;
        }

        public static DateTime GetBuildDate(Assembly? assembly)
        {
            if (assembly == null)
            {
                return default;
            }

            var attribute = assembly.GetCustomAttribute<BuildDateAttribute>();
            return attribute != null ? attribute.DateTime : default(DateTime);
        }

        public static DateTime GetBuildDate()
        {
            return GetBuildDate(Assembly.GetEntryAssembly());
        }
    }
}
