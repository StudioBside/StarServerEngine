namespace Cs.Core.Util
{
    using System;
    using System.Globalization;

    // https://www.meziantou.net/getting-the-date-of-build-of-a-dotnet-assembly-at-runtime.htm
    [AttributeUsage(AttributeTargets.Assembly)]
    public class BuildDateAttribute : Attribute
    {
        public BuildDateAttribute(string value)
        {
            this.DateTime = DateTime.ParseExact(value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None);
        }

        public DateTime DateTime { get; }
    }
}
