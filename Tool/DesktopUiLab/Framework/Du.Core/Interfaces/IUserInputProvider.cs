namespace Du.Core.Interfaces;

using System.Threading.Tasks;

/// <summary>
/// 사용자에게 UI를 통해 T 타입의 입력을 받는 역할.
/// </summary>
/// <typeparam name="T">입력받는 데이터의 타입.</typeparam>
public interface IUserInputProvider<T>
{
    Task<T?> PromptAsync(string title, string message);
    Task<T?> PromptAsync(string title, string message, T defaultValue);
}
