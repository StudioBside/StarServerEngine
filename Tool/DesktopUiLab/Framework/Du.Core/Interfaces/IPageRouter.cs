namespace Du.Core.Interfaces;

public interface IPageRouter
{
    event EventHandler<object> Navigated;

    void Route(object? parameter);
}
