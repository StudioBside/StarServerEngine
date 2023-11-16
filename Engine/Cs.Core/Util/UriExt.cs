namespace Cs.Core.Util
{
    using System;

    public static class UriExt
    {
        public static Uri Append(this Uri uri, params string[] segments)
        {
            var absoluteUri = uri.AbsoluteUri.AppendToURL(segments);
            return new Uri(absoluteUri);
        }
    }
}
