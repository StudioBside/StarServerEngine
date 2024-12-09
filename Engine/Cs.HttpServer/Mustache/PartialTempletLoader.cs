namespace Cs.HttpServer.Mustache;

using System.IO;
using System.Text;
using System.Threading.Tasks;
using Stubble.Core.Interfaces;

internal sealed class PartialTempletLoader : IStubbleLoader
{
    private readonly string basePath;

    public PartialTempletLoader(string basePath)
    {
        this.basePath = basePath;
    }

    public IStubbleLoader Clone()
    {
        return new PartialTempletLoader(this.basePath);   
    }

    public string Load(string name)
    {
        string fullPath = Path.Combine(this.basePath, name);
        using (var streamReader = new StreamReader(fullPath, Encoding.UTF8))
        {
            return streamReader.ReadToEnd();
        }
    }

    public async ValueTask<string> LoadAsync(string name)
    {
        string fullPath = Path.Combine(this.basePath, name);
        using (var streamReader = new StreamReader(fullPath, Encoding.UTF8))
        {
            return await streamReader.ReadToEndAsync();
        }
    }
}
