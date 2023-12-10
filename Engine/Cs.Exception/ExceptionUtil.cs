namespace Cs.Exception
{
    using System;
    using System.IO;
    using System.Text;

    public static class ExceptionUtil
    {
        public static string FlattenInnerExceptions(Exception? root)
        {
            var builder = new StringBuilder();
            using (var writer = new StringWriter(builder))
            {
                DumpExceptions(root, writer, depth: 0);
            }

            return builder.ToString();
        }

        public static void FlattenInnerExceptions(Exception root, StringWriter writer)
        {
            DumpExceptions(root, writer, depth: 0);
        }

        public static bool HasException(Exception? exception, Type targetType)
        {
            if (exception == null)
            {
                return false;
            }

            if (exception.GetType() == targetType)
            {
                return true;
            }

            if (exception is AggregateException aggregate)
            {
                foreach (var inner in aggregate.InnerExceptions)
                {
                    if (HasException(inner, targetType))
                    {
                        return true;
                    }
                }
            }

            return HasException(exception.InnerException, targetType);
        }

        private static void DumpExceptions(Exception? exception, StringWriter writer, int depth)
        {
            if (exception == null)
            {
                return;
            }

            writer.WriteLine($"**** depth:{depth} {exception.GetType().Name} message:{exception.Message}");
            writer.WriteLine(exception.StackTrace);

            if (exception is AggregateException aggregate)
            {
                foreach (var inner in aggregate.InnerExceptions)
                {
                    DumpExceptions(inner, writer, depth + 1);
                }
            }
            else
            {
                DumpExceptions(exception.InnerException, writer, depth + 1);
            }
        }
    }
}
