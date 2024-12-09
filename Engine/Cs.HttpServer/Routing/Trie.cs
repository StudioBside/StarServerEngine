namespace Cs.HttpServer.Routing;

using System.Collections.Generic;

internal sealed partial class Trie
{
    private readonly Node root = new Node(string.Empty);

    public Trie()
    {
    }

    public bool Add(RequestHandler handler)
    {
        return this.root.AddHandler(handler.Uri.Split('/'), tokenIndex: 0, handler);
    }

    public RequestHandler? Find(string uri, List<object> handlerArgs)
    {
        return this.root.MatchHandler(uri.Split('/'), tokenIndex: 0, handlerArgs);
    }
}
