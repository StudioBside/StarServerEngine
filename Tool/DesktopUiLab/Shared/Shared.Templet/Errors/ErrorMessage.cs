namespace Shared.Templet.Errors;

public enum ErrorType
{
    All,
    Unit,
    Skill,
    Buff,
    String,
}

public sealed record ErrorMessage(ErrorType ErrorType, string Message, object? Target)
{
    public static void Add(ErrorType errorType, string message)
    {
        ErrorMessageController.Instance.Add(new(errorType, message, Target: null));
    }

    public static void Add(ErrorType errorType, string message, object target)
    {
        ErrorMessageController.Instance.Add(new(errorType, message, target));
    }
}
