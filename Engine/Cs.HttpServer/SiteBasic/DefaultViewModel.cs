namespace Cs.HttpServer.SiteBasic
{
    using Cs.Core.Util;

    public sealed class DefaultViewModel
    {
        public VersionData Version { get; } = new VersionData();
        public string BuildTime { get; } = LinkerTime.GetBuildDate().ToString("yyyy-MM-dd HH:mm:ss");
    }
}
