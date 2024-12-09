namespace Cs.HttpServer.Mustache;

using System.IO;
using Stubble.Core;
using ReadOnlyViewModel = System.Collections.Generic.IReadOnlyDictionary<string, object>;

public interface IMustacheSource
{
    public void Render(
        StubbleVisitorRenderer stubble,
        ReadOnlyViewModel viewModel,
        RenderChain.Node? next,
        StreamWriter writer);
}
