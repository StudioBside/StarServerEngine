namespace Cs.HttpServer;

using System;
using System.IO;
using Newtonsoft.Json;

public sealed class HttpFrameworkConfig
{
    [JsonConstructor]
    public HttpFrameworkConfig(ushort port, bool verbose, string frontPath)
    {
        this.Port = port;
        this.Verbose = verbose;
        this.FrontPath = frontPath;
    }

    public ushort Port { get; }
    public bool Verbose { get; }
    public string FrontPath { get; set; }
}
