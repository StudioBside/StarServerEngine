namespace Shared.Templet.Errors;

using System;

public enum ErrorType
{
    All,
    Unit,
    String,
}

public sealed record ErrorMessage(ErrorType ErrorType, string Message);
