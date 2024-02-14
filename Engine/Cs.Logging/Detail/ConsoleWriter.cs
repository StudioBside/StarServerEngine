namespace Cs.Logging.Detail
{
    using System;
    using static Cs.Logging.Log;

    internal static class ConsoleWriter
    {
        public static void PutLog(LogLevel level, string message, ConsoleColor? color = null)
        {
            if (color.HasValue)
            {
                Console.ForegroundColor = color.Value;
            }
            else
            {
                switch (level)
                {
                    case LogLevel.Info:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;

                    case LogLevel.Debug:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;

                    case LogLevel.Warn:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;

                    case LogLevel.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                }
            }

            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}