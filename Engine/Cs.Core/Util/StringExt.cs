namespace Cs.Core.Util
{
    using System;
    using System.Linq;
    using System.Text;

    public static class StringExt
    {
        public static string ToByteFormat(this ushort bytes)
        {
            string[] suffix = new string[] { "b", "Kb", "Mb", "Gb", "Tb", "Pb", "Eb", "Zb", "Yb" };

            long before = 0;
            long current = bytes;
            int index = 0;

            while (current > 1024)
            {
                before = current & 0b11_1111_1111;
                current = current >> 10;
                ++index;
            }

            if (before > 100)
            {
                return $"{current}.{before / 10:##}{suffix[index]}";
            }

            return $"{current}{suffix[index]}";
        }

        public static string ToByteFormat(this int bytes)
        {
            string[] suffix = new string[] { "b", "Kb", "Mb", "Gb", "Tb", "Pb", "Eb", "Zb", "Yb" };

            long before = 0;
            long current = bytes;
            int index = 0;

            while (current > 1024)
            {
                before = current & 0b11_1111_1111;
                current = current >> 10;
                ++index;
            }

            if (before > 100)
            {
                return $"{current}.{before / 10:##}{suffix[index]}";
            }

            return $"{current}{suffix[index]}";
        }

        public static string ToByteFormat(this long bytes)
        {
            string[] suffix = new string[] { "b", "Kb", "Mb", "Gb", "Tb", "Pb", "Eb", "Zb", "Yb" };

            long before = 0;
            long current = bytes;
            int index = 0;

            while (current > 1024)
            {
                before = current & 0b11_1111_1111;
                current = current >> 10;
                ++index;
            }

            if (before > 100)
            {
                return $"{current}.{before / 10:##}{suffix[index]}";
            }

            return $"{current}{suffix[index]}";
        }

        public static string ToByteFormat(this ulong bytes)
        {
            string[] suffix = new string[] { "b", "Kb", "Mb", "Gb", "Tb", "Pb", "Eb", "Zb", "Yb" };

            ulong before = 0;
            ulong current = bytes;
            int index = 0;

            while (current > 1024)
            {
                before = current & 0b11_1111_1111;
                current = current >> 10;
                ++index;
            }

            if (before > 100)
            {
                return $"{current}.{before / 10:##}{suffix[index]}";
            }

            return $"{current}{suffix[index]}";
        }

        public static string ToTimeFormat(this long msec)
        {
            if (msec < 1000)
            {
                // 1초 미만
                return $"{msec.ToString()}ms";
            }

            float sec = msec / 1000f;
            if (sec < 60)
            {
                // 1분 미만
                return $"{sec:.##}s";
            }

            var span = TimeSpan.FromMilliseconds(msec);
            if (span.TotalHours < 1)
            {
                // 1시간 미만
                return span.ToString(@"mm\:ss\.ff");
            }

            return span.ToString(@"hh\:mm\:ss\.ff"); // 1시간 이상
        }

        public static string EncodeBase64(this string data) => Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
        public static string EncodeBase64(this byte[] data) => Convert.ToBase64String(data);
        public static string DecodeBase64(this string data) => Encoding.UTF8.GetString(Convert.FromBase64String(data));
        public static byte[] DecodeBase64ToBytes(this string data) => Convert.FromBase64String(data);

        public static string DecodeBase64Url(this string data)
        {
            string base64 = data.Replace('-', '+').Replace('_', '/');
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            byte[] bytes = Convert.FromBase64String(base64);
            return Encoding.UTF8.GetString(bytes);
        }

        public static string CalcMd5Checksum(this string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }

            using var md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(data);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }

            return sb.ToString();
        }

        public static string DisplayWithSuffix(this int num)
        {
            string number = num.ToString();
            if (number.EndsWith("11") ||
                number.EndsWith("12") ||
                number.EndsWith("13"))
            {
                return number + "th";
            }

            if (number.EndsWith("1"))
            {
                return number + "st";
            }

            if (number.EndsWith("2"))
            {
                return number + "nd";
            }

            if (number.EndsWith("3"))
            {
                return number + "rd";
            }

            return number + "th";
        }

        public static string ToUtf8(this string data, Encoding currentEncoding)
        {
            // encode the string as an ASCII byte array
            byte[] asciiBytes = currentEncoding.GetBytes(data);

            // convert the ASCII byte array to a UTF-8 byte array
            byte[] utf8Bytes = Encoding.Convert(currentEncoding, Encoding.UTF8, asciiBytes);

            // reconstitute a string from the UTF-8 byte array 
            return Encoding.UTF8.GetString(utf8Bytes);
        }

        public static string AppendToURL(this string baseURL, params string[] segments)
        {
            return string.Join("/", new[] { baseURL.TrimEnd('/') }
                .Concat(segments.Select(s => s.Trim('/'))));
        }
    }
}
