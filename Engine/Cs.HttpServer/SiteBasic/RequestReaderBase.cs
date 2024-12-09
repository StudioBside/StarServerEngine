namespace Cs.HttpServer.SiteBasic;

using System.Net;

public abstract class RequestReaderBase
{
    private readonly HttpListenerRequest request;

    protected RequestReaderBase(HttpListenerRequest request)
    {
        this.request = request;
    }

    public bool UrlIsHome => this.request.Url?.AbsolutePath == "/";
    protected HttpListenerRequest Request => this.request;

    public override string ToString() => $"{this.request.HttpMethod} {this.request.Url?.AbsolutePath}";
}
