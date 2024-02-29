namespace Cs.Repl.Detail;

using System.Reflection;
using System.Threading.Tasks;

internal sealed class Command(string name, string description, MethodInfo method)
{
    private readonly MethodInfo method = method;
    public string Name { get; } = name;
    public string Description { get; } = description;
    
    public async Task<string> Invoke(object instance, string argument)
    {
        var untyped = this.method.Invoke(instance, new[] { argument });
        return untyped switch
        {
            Task<string> task => await task,
            string str => str,
            null => $"command return null object.",
            _ => $"unsupported return type:{untyped.GetType().Name}",
        };
    }
}