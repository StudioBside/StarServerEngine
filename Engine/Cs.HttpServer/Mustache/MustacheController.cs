namespace Cs.HttpServer.Mustache
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Cs.HttpServer;
    using Cs.HttpServer.SiteBasic;
    using Cs.Logging;
    using Stubble.Core.Builders;
    using ViewModel = System.Collections.Generic.Dictionary<string, object>;

    public sealed class MustacheController
    {
        private readonly ViewModel viewModel = new();
        private readonly StubbleBuilder stubbleBuilder = new();
        private SourceLayout globalLayout = null!;
        private string pagePath = string.Empty;
        private Func<HttpListenerRequest, RequestReaderBase> requestReaderFactory = null!;

        public bool Initialize(HttpFrameworkConfig config)
        {
            this.pagePath = Path.Combine(config.FrontPath, "Pages");
            if (Directory.Exists(this.pagePath) == false)
            {
                ErrorContainer.Add($"page path not exist:{this.pagePath}");
                return false;
            }

            var sharedPagePath = Path.Combine(this.pagePath, "Shared");
            if (Directory.Exists(sharedPagePath) == false)
            {
                ErrorContainer.Add($"shared page path not exist:{sharedPagePath}");
                return false;
            }

            var layoutFilePath = Directory.GetFiles(sharedPagePath, "*.html", SearchOption.AllDirectories)
                .FirstOrDefault(e => e.EndsWith("layout.html", StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrEmpty(layoutFilePath))
            {
                ErrorContainer.Add($"layout.html not exist in shared page path:{sharedPagePath}");
                return false;
            }

            var layout = SourceLayout.Create(layoutFilePath);
            if (layout is null)
            {
                ErrorContainer.Add($"layout.html load failed:{layoutFilePath}");
                return false;
            }

            this.globalLayout = layout;

            this.AddViewModel("default", new DefaultViewModel());

            var partialLoader = new PartialTempletLoader(this.pagePath);
            this.stubbleBuilder.Configure(settings => settings.SetPartialTemplateLoader(partialLoader));

            return true;
        }

        public void AddViewModel(string key, object value)
        {
            this.viewModel[key] = value;
        }

        public bool HasViewModel(string key)
        {
            return this.viewModel.ContainsKey(key);
        }

        public void SetRequestReaderFactory(Func<HttpListenerRequest, RequestReaderBase> requestReaderFactory)
        {
            this.requestReaderFactory = requestReaderFactory;
        }

        public void Render(string filePath, HttpListenerContext context)
        {
            var fullPath = Path.Combine(this.pagePath, filePath);
            var sourcePage = SourcePage.Create(fullPath);
            if (sourcePage == null)
            {
                context.SetResponseJson(HttpStatusCode.NotFound, $"{{ error: \"[Mustache] page not found. filePath:{filePath}\"}}");
                return;
            }

            // 매번 요청마다 request reader를 갱신해준다. 
            this.AddViewModel("request", this.requestReaderFactory(context.Request));

            var response = context.Response;
            response.ContentType = "text/html";
            response.ContentEncoding = Encoding.UTF8;
            response.StatusCode = (int)HttpStatusCode.OK;

            var stubble = this.stubbleBuilder.Build();
            var renderChain = new RenderChain(this.globalLayout, sourcePage.SubLayout, sourcePage);
            renderChain.StartRender(stubble, this.viewModel, context.Response);
        }
    }
}
