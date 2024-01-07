namespace Cs.HttpServer
{
    public static class Enums
    {
        public enum MethodType
        {
            Get,
            Post,
            Count,
        }

        internal enum ParamType
        {
            Byte,
            Short,
            Int,
            Long,
            String,
            Boolean,
            DateTime,
            TimeSpan,
        }
    }
}
