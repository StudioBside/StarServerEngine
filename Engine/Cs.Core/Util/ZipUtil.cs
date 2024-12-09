namespace Cs.Core.Util;

using System.IO;
using System.IO.Compression;

public static class ZipUtil
{
    public static byte[] Compress(byte[] source)
    {
        using (var outputStream = new MemoryStream())
        using (var compressor = new GZipStream(outputStream, CompressionMode.Compress, true))
        {
            compressor.Write(source, 0, source.Length);

            return outputStream.ToArray();
        }
    }

    public static byte[] Decompress(byte[] source)
    {
        using (var outputStream = new MemoryStream())
        using (var decompressor = new GZipStream(new MemoryStream(source), CompressionMode.Decompress))
        {
            byte[] buffer = new byte[4096];
            int read_count = 0;
            while ((read_count = decompressor.Read(buffer, 0, buffer.Length)) != 0)
            {
                outputStream.Write(buffer, 0, read_count);
            }

            return outputStream.ToArray();
        }
    }
}
