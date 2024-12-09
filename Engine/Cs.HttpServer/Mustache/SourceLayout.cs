namespace Cs.HttpServer.Mustache;

using System.IO;
using System.Text;
using Cs.Logging;
using Stubble.Core;
using ReadOnlyViewModel = System.Collections.Generic.IReadOnlyDictionary<string, object>;

internal sealed class SourceLayout : IMustacheSource
{
    private readonly string fullPath = string.Empty;

    private SourceLayout(string fullPath)
    {
        this.fullPath = fullPath;
    }

    public static SourceLayout? Create(string fullPath)
    {
        if (File.Exists(fullPath) == false)
        {
            ErrorContainer.Add($"[Mustache] invalid layoutPath. fullPath:{fullPath}");
            return null;
        }

        return new SourceLayout(fullPath);
    }

    public void Render(
        StubbleVisitorRenderer stubble,
        ReadOnlyViewModel viewModel,
        RenderChain.Node? next,
        StreamWriter writer)
    {
        if (next == null)
        {
            Log.Error($"[Mustach] layout에는 next node를 필수 연결해야 합니다. fullPath:{this.fullPath}");
        }

        string layoutSource;

        // 매번 요청할 때마다 새로 불러와야 개발중 수정사항이 바로 반영된다.
        using (var streamReader = new StreamReader(this.fullPath, Encoding.UTF8))
        {
            layoutSource = streamReader.ReadToEnd();
        }

        // find '@Body' in this.body
        var bodyIndex = layoutSource.IndexOf("@Body", System.StringComparison.Ordinal);
        if (bodyIndex < 0)
        {
            ErrorContainer.Add($"[Mustache] invalid layout. no '@body' keyword in layout. fullPath:{this.fullPath}");
            return;
        }

        writer.Write(stubble.Render(layoutSource[..bodyIndex], viewModel));
        if (next != null)
        {
            next.Source.Render(stubble, viewModel, next.Next, writer);
        }

        writer.Write(stubble.Render(layoutSource[(bodyIndex + 5)..], viewModel));
    }
}
