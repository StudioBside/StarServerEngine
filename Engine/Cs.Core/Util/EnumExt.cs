namespace Cs.Core.Util
{
    using System;
    using System.ComponentModel;
    using System.Linq;

    public static class EnumExt
    {
        public static T Parse<T>(this string data)
        {
            return (T)Enum.Parse(typeof(T), data, true);
        }

        public static bool TryParse<T>(this string data, out T @enum) where T : struct, Enum
        {
            return Enum.TryParse(data, true, out @enum);
        }

        public static string GetDescription<T>(this T enumValue) where T : Enum
        {
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            if (fieldInfo == null)
            {
                return enumValue.ToString();
            }

            var attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), inherit: false) as DescriptionAttribute[];
            if (attributes != null && attributes.Any())
            {
                return attributes.Where(e => string.IsNullOrEmpty(e.Description) == false).First().Description;
            }

            return enumValue.ToString();
        }

        public static string GetEnumDescription<T>(this string value) where T : struct, Enum
        {
            if (value.TryParse<T>(out T enumValue) == false)
            {
                return value;
            }

            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            if (fieldInfo == null)
            {
                return enumValue.ToString();
            }

            var attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), inherit: false) as DescriptionAttribute[];
            if (attributes != null && attributes.Any())
            {
                return attributes.Where(e => string.IsNullOrEmpty(e.Description) == false).First().Description;
            }

            return enumValue.ToString();
        }
    }
}
