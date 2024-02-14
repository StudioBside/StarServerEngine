namespace GptPlay.Main;

using System;
using Cs.Gpt;
using Cs.Repl;
using static Cs.Gpt.GptTranslator;

internal sealed class InputHandler : ReplHandlerBase, IDisposable
{
    private readonly ReplConsole console;
    private readonly GptTranslator client;

    public InputHandler(ReplConsole console, GptPlayConfig config)
    {
        this.console = console;
        this.client = new GptTranslator(config.ApiKey, ServiceMode.TranslateToChinese);
    }

    public void Dispose()
    {
        this.client.Dispose();
    }

    public override Task<string> Evaluate(string input)
    {
        return this.client.GetResponse(input);
    }
}
