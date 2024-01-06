namespace Cs.HttpServer.SiteBasic
{
    using Cs.Core.Util;

    public readonly struct VersionData
    {
        public VersionData()
        {
            this.Stream = DevOps.BuildInformation.StreamName;
            this.Revision = DevOps.BuildInformation.Revision;
        }

        public string Stream { get; }
        public int Revision { get; }

        public override string ToString() => $"{this.Stream}@{this.Revision}";
    }
}
