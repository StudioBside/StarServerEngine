namespace Cs.HttpServer.Mustache;

using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Stubble.Core;
using ReadOnlyViewModel = System.Collections.Generic.IReadOnlyDictionary<string, object>;

public readonly struct RenderChain
{
    private readonly Node rootNode;

    public RenderChain(params IMustacheSource?[] sources)
    {
        Node node = null!;

        foreach (var source in sources.Reverse())
        {
            if (source is null)
            {
                continue;
            }

            node = new Node(source, node);
        }

        this.rootNode = node ?? throw new System.ArgumentException("sources is empty");
    }

    public void StartRender(StubbleVisitorRenderer stubble, ReadOnlyViewModel viewModel, HttpListenerResponse response)
    {
        using (var streamWriter = new StreamWriter(response.OutputStream, Encoding.UTF8))
        {
            this.rootNode.Source.Render(stubble, viewModel, this.rootNode.Next, streamWriter);
        }

        response.Close();
    }

    public sealed record Node(IMustacheSource Source, Node? Next);
}
