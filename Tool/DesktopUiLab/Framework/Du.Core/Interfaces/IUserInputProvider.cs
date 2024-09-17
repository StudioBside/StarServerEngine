namespace Du.Core.Interfaces;

using System.Threading.Tasks;

public interface IUserInputProvider<T>
{
    Task<T?> PromptAsync(string message);
    Task<T?> PromptAsync(string message, T defaultValue);
}
