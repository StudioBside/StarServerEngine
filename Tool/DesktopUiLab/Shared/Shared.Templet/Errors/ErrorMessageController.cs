namespace Shared.Templet.Errors;

using System.Collections.Generic;
using Cs.Core;

public sealed class ErrorMessageController
{
    private readonly List<ErrorMessage> list = new();

    public static ErrorMessageController Instance => Singleton<ErrorMessageController>.Instance;
    public IEnumerable<ErrorMessage> List => this.list;
    public int Count => this.list.Count;

    public void Add(ErrorMessage message)
    {
        this.list.Add(message);
    }
}
