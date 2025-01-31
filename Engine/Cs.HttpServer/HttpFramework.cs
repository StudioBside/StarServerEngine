namespace Cs.HttpServer;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Cs.Exception;
using Cs.HttpServer.Mustache;
using Cs.HttpServer.Routing;
using Cs.Logging;
using static Cs.HttpServer.Enums;

public sealed class HttpFramework : IDisposable
{
    private static readonly Dictionary<string, string> StaticFileExtentions;
    private readonly HttpListener listener = new();
    private readonly Router router = new();
    private readonly MustacheController mustacheController = new();
    private HttpFrameworkConfig config = null!;
    private string staticPath = string.Empty;

    static HttpFramework()
    {
        StaticFileExtentions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "png", "image/png" },
            { "jpg", "image/jpg" },
            { "jpeg", "image/jpg" },
            { "gif", "image/gif" },
            { "ico", "image/x-icon" },
            { "css", "text/css" },
            { "map", "application/x-navimap" },
            { "js", "application/x-javascript" },
            { "ttf", "application/x-font-ttf" },
            { "woff", "application/x-font-woff" },
            { "woff2", "application/font-woff2" },
            { "eot", "application/vnd.ms-fontobject" },
        };
    }

    public MustacheController MustacheController => this.mustacheController;

    public void RegisterController(Type controllerType)
    {
        this.router.RegisterController(controllerType);
    }

    public bool Initialize(in HttpFrameworkConfig config)
    {
        this.config = config;
        this.RegisterUrlAcl(config.Port);

        if (string.IsNullOrEmpty(config.FrontPath) == false)
        {
            this.staticPath = Path.Combine(config.FrontPath, "wwwRoot");
            if (Directory.Exists(this.staticPath) == false)
            {
                Log.Error($"static path not exist:{this.staticPath}");
            }
            else
            {
                Log.Info($"[HttpFramework] static path:{this.staticPath}");
            }

            if (this.mustacheController.Initialize(config) == false)
            {
                return false;
            }
        }

        try
        {
            this.listener.Start();
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            return false;
        }
    }

    public async Task ProcessAsync()
    {
        var context = await this.listener.GetContextAsync();
        while (context != null)
        {
            ThreadPool.QueueUserWorkItem(async _ =>
            {
                try
                {
                    await this.OnRequest(context);
                }
                catch (Exception ex)
                {
                    string exceptionDetail = ExceptionUtil.FlattenInnerExceptions(ex);
                    Log.Error(exceptionDetail);
                    context.SetResponseJsonError(HttpStatusCode.InternalServerError, exceptionDetail);
                }
            });

            context = await this.listener.GetContextAsync();
        }
    }

    public void Dispose()
    {
        ((IDisposable)this.listener).Dispose();
    }

    private void RegisterUrlAcl(int port)
    {
        this.listener.Prefixes.Add($"http://+:{port}/");

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) != false)
        {
            var urlAclList = new UrlAclList();
            foreach (var data in this.listener.Prefixes)
            {
                if (urlAclList.Contains(data))
                {
                    continue;
                }

                Log.Info($"Add url prefix to netsh urlacl list: {data}");
                var myProcessInfo = new ProcessStartInfo("netsh", $"http add urlacl url=\"{data}\" user=everyone")
                {
                    Verb = "runas",
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = true,
                };
                var process = Process.Start(myProcessInfo);
                process?.WaitForExit();
            }
        }
    }

    private async Task OnRequest(HttpListenerContext context)
    {
        var request = context.Request;
        if (!Enum.TryParse(request.HttpMethod, ignoreCase: true, out MethodType methodType))
        {
            Log.Error($"unsupported method type:{methodType}");
            context.SetResponseHtml($"<html><body>unsupported method type:{methodType}</body></html>");
            return;
        }

        if (this.config.Verbose)
        {
            Log.Info($"{request.HttpMethod} {request.Url?.AbsolutePath} {request.Url?.Query}");
        }

        if (await this.TrySendStaticFile(methodType, request.RawUrl ?? string.Empty, context))
        {
            return;
        }

        bool result = await this.router.HandleAsync(context);
        if (!result)
        {
            context.SetResponseHtml("<html><body>Routing Error!</body></html>");
        }
    }

    private async Task<bool> TrySendStaticFile(MethodType methodType, string uri, HttpListenerContext context)
    {
        if (methodType != MethodType.Get)
        {
            return false;
        }

        int position = uri.LastIndexOf('.');
        if (position < 0)
        {
            return false;
        }

        string extension = uri[(position + 1)..];
        if (StaticFileExtentions.TryGetValue(extension, out var mimeType) == false)
        {
            return false;
        }

        // uri의 첫 문자 '/'를 제거하지 않으면 path를 조합할 때 root path로 처리된다.
        string fullPath = Path.GetFullPath(Path.Combine(this.staticPath, uri[1..]));
        if (!File.Exists(fullPath))
        {
            return false;
        }

        FileStream fileStream = File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        await context.SetResponseFileAsync(fileStream, mimeType);
        return true;
    }
}
