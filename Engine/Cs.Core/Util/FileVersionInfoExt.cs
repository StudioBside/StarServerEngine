namespace Cs.Core.Util
{
    using System.Diagnostics;
    using static Cs.Core.Util.DevOps;

    public static class FileVersionInfoExt
    {
        public static int GetStreamId(this FileVersionInfo self) => self.FileMajorPart;
        public static int GetRevision(this FileVersionInfo self) => (self.FileMinorPart * 10000) + self.FileBuildPart;
        public static int GetProtocolVersion(this FileVersionInfo self) => self.FilePrivatePart;

        public static string GetStreamName(this FileVersionInfo self)
        {
            var streamType = (P4StreamType)self.FileMajorPart;
            return streamType.ToString();
        }
    }
}
