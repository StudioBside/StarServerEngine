namespace Cs.Repl.Detail;

using System.Reflection;

internal sealed class Command(string name, string description, MethodInfo method)
{
    public string Name { get; } = name;
    public string Description { get; } = description;
    
    public string Invoke(object instance, string argument)
    {
        var result = method.Invoke(instance, new[] { argument });
        if (result is not string strResult)
        {
            return "Invalid command result.";
        }

        return strResult;
    }
}