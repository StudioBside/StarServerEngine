namespace Cs.Repl;

using System;
using System.Threading.Tasks;

public sealed class ReplConsole
{
    public string Prompt { get; set; } = "REPL";
    public ReplHandlerBase Handler { get; private set; } = null!;

    public void Initialize(ReplHandlerBase handler)
    {
        this.Handler = handler;
    }

    public async Task Run()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine($"Type 'exit' to end.");

        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{this.Prompt}> ");

            Console.ForegroundColor = ConsoleColor.White;
            string input = Console.ReadLine() ?? string.Empty;

            if (input.Equals("exit", StringComparison.CurrentCultureIgnoreCase))
            {
                break;
            }

            if (string.IsNullOrEmpty(input))
            {
                continue;
            }

            try
            {
                var result = await this.Handler.Evaluate(input);

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
