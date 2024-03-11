namespace Cs.Repl.Detail;

using System.Reflection;
using System.Threading.Tasks;

internal sealed class Command
{
    private readonly RawHandlerType rawHandler;

    public Command(string name, string description, RawHandlerType rawHandler)
    {
        this.Name = name;
        this.Description = description;
        this.rawHandler = rawHandler;
    }

    internal delegate Task<string> RawHandlerType(ReplHandlerBase handlerBase, string argument);
    
    public string Name { get; }
    public string Description { get; }
    
    public async Task<string> Invoke(ReplHandlerBase handlerBase, string argument)
    {
        return await this.rawHandler.Invoke(handlerBase, argument);
    }
}