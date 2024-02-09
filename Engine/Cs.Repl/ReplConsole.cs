namespace Cs.Repl;

using System;

public sealed class ReplConsole(string name, ReplHandlerBase handler)
{
    public void Run()
    {
        Console.WriteLine($"Type 'exit' to end.");

        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{name}> ");

            Console.ForegroundColor = ConsoleColor.White;
            string input = Console.ReadLine() ?? string.Empty;

            if (input.Equals("exit", StringComparison.CurrentCultureIgnoreCase))
            {
                break;
            }

            try
            {
                var result = handler.Evaluate(input);

                Console.WriteLine();
                Console.WriteLine(result);
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
