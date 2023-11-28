namespace Cs.Core.Util
{
    using System;
    using System.Xml.Linq;
    using System.Xml.XPath;

    public static class XmlExt
    {
        public static T Get<T>(this XElement element, string name)
        {
            return (T)GetValue(element, name, typeof(T));
        }

        public static T TryGet<T>(this XElement element, string name, T default_value)
        {
            object? result = TryGetValue(element, name, typeof(T), default_value);
            if (result == null)
            {
                return default_value;
            }

            return (T)result;
        }

        public static T GetAttribute<T>(this XElement element, string name)
        {
            object result = GetAttribute(element, name, typeof(T));
            return (T)result;
        }

        public static T TryGetAttribute<T>(this XElement element, string name, T default_value)
        {
            object? result = TryGetAttribute(element, name, typeof(T), default_value);
            if (result == null)
            {
                return default_value;
            }

            return (T)result;
        }

        public static T XPathGet<T>(this XElement element, string name)
        {
            var result = element.XPathSelectElement(name);
            return (T)Convert(result?.Value ?? string.Empty, typeof(T));
        }

        private static object GetValue(XElement element, string name, Type type)
        {
            string value = element.Element(name)?.Value ?? string.Empty;
            return Convert(value, type);
        }

        private static object? TryGetValue(XElement element, string name, Type type, object? default_value)
        {
            XElement? sub_element = element.Element(name);
            if (sub_element == null || String.IsNullOrEmpty(sub_element.Value))
            {
                return default_value;
            }

            return Convert(sub_element.Value, type);
        }

        private static object GetAttribute(XElement element, string name, Type data_type)
        {
            var attr = element.Attribute(name);
            return Convert(attr?.Value ?? string.Empty, data_type);
        }

        private static object? TryGetAttribute(XElement element, string name, Type data_type, object? default_value)
        {
            var attr = element.Attribute(name);
            if (attr == null)
            {
                return default_value;
            }

            return Convert(attr.Value, data_type);
        }

        private static object Convert(string value, Type data_type)
        {
            if (data_type.IsEnum)
            {
                return Enum.Parse(data_type, value, true);
            }

            switch (Type.GetTypeCode(data_type))
            {
                case TypeCode.String:
                    return value;
                case TypeCode.Int32:
                    return Int32.Parse(value);
                case TypeCode.Int64:
                    return Int64.Parse(value);
                case TypeCode.Boolean:
                    return Boolean.Parse(value);
                case TypeCode.DateTime:
                    return DateTime.Parse(value);
                case TypeCode.Single:
                    return Single.Parse(value);
                case TypeCode.Double:
                    return Double.Parse(value);
            }

            throw new NotImplementedException();
        }
    }
}
