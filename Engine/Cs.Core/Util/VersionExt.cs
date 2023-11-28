namespace Cs.Core.Util
{
    using System;

    public static class VersionExt
    {
        public static int GetStreamId(this Version? self)
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }

            return self.Major;
        }

        public static int GetCustomRevision(this Version? self)
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }

            return (self.Minor * 10000) + self.Build;
        }

        public static int GetProtocolVersion(this Version? self)
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }

            return self.Revision;
        }
    }
}
