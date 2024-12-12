namespace Du.Core.Interfaces;

using System.Threading.Tasks;

/// <summary>
/// 2개의 응답을 받을 때는 IUserInputProvider[bool]을 사용.
/// 3개의 응답을 받을 때는 IDialogProvider를 사용합니다.
/// </summary>
public interface IDialogProvider
{
    public enum ResultType
    {
        None,
        Primary,
        Secondary,
    }

    ResultType Show(string title, string message);

    Task<ResultType> ShowAsync(
        string title,
        string message,
        string btnText1,
        string btnText2,
        string btnText3);
}
