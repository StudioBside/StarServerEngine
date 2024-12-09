namespace Cs.HttpServer.Mustache;

using System;
using System.IO;
using System.Text;
using Cs.Logging;
using Stubble.Core;
using ReadOnlyViewModel = System.Collections.Generic.IReadOnlyDictionary<string, object>;

internal sealed class SourcePage : IMustacheSource
{
    private readonly string fullPath;
    private readonly SourceLayout? subLayout;

    private SourcePage(string fullPath, SourceLayout? subLayout)
    {
        this.fullPath = fullPath;
        this.subLayout = subLayout;
    }

    public SourceLayout? SubLayout => this.subLayout;

    public static SourcePage? Create(string fullPath)
    {
        if (File.Exists(fullPath) == false)
        {
            ErrorContainer.Add($"[Mustache] invalid layoutPath. fullPath:{fullPath}");
            return null;
        }

        // 동일한 폴더에 layout이 존재하는지 확인한다.
        var subLayoutPath = Path.GetDirectoryName(fullPath) ?? throw new System.Exception($"invalid filePath:{fullPath}");
        SourceLayout? subLayout = null;
        foreach (var file in Directory.GetFiles(subLayoutPath, "*.html"))
        {
            if (file.EndsWith("layout.html", StringComparison.OrdinalIgnoreCase))
            {
                subLayout = SourceLayout.Create(file);
                break;
            }
        }

        return new SourcePage(fullPath, subLayout);
    }

    public void Render(
        StubbleVisitorRenderer stubble,
        ReadOnlyViewModel viewModel,
        RenderChain.Node? next,
        StreamWriter writer)
    {
        if (next != null)
        {
            Log.Error($"[Mustach] page에는 next nodoe를 연결할 수 없습니다. fullPath:{this.fullPath}");
        }

        string pageSource;

        // 매번 요청할 때마다 새로 불러와야 개발중 수정사항이 바로 반영된다.
        using (var streamReader = new StreamReader(this.fullPath, Encoding.UTF8))
        {
            pageSource = streamReader.ReadToEnd();
        }

        writer.Write(stubble.Render(pageSource, viewModel));
    }
}
