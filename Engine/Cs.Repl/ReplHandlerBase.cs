namespace Cs.Repl;

using System.Threading.Tasks;

public abstract class ReplHandlerBase
{
    public abstract Task<string> Evaluate(string input);
}
