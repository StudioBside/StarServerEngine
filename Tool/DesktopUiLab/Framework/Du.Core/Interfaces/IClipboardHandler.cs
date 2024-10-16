namespace Du.Core.Interfaces;

using System.Threading.Tasks;

public interface IClipboardHandler
{
    Task<bool> HandlePastedTextAsync(string text);
}
