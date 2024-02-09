namespace GptPlay.Main;

using System;
using Cs.Repl;

internal sealed class InputHandler : ReplHandlerBase
{
    public override string Evaluate(string input)
    {
        return input;
    }
}
