namespace Cs.HttpServer;

using System;
using System.Runtime.CompilerServices;
using Cs.Core.Util;
using static Cs.HttpServer.Enums;

[AttributeUsage(AttributeTargets.Method)]
public sealed class RouteUrlAttribute : Attribute
{
    public RouteUrlAttribute(MethodType methodType, string url, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
    {
        this.Method = methodType;
        this.Url = url;
        this.FileLine = TagBuilder.Build(file, line);
    }

    public string Url { get; }
    public MethodType Method { get; }
    public string FileLine { get; }
}
