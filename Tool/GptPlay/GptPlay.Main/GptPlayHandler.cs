namespace GptPlay.Main;

using System;
using Cs.Gpt;
using Cs.Repl;
using static Cs.Gpt.GptTranslator;

internal sealed class GptPlayHandler(GptPlayConfig config) : ReplHandlerBase, IDisposable
{
    private readonly GptTranslator client = new GptTranslator(config.ApiKey);

    public void Dispose()
    {
        this.client.Dispose();
    }

    public override Task<string> Evaluate(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return Task.FromResult(string.Empty);
        }

        return this.client.Translate(TranslationMode.ToEnglish, input);
    }
}
