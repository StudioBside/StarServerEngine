namespace Du.Core.Interfaces;

using System.Threading.Tasks;

public interface IKeyDownHandler
{
    Task<bool> HandleKeyDownAsync(char key, bool ctrl, bool shift, bool alt);
}
