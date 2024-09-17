namespace Du.WpfLib;

using System.Threading.Tasks;
using System.Windows.Threading;
using Du.Core.Interfaces;

public sealed class StringInputProvider : IUserInputProvider<string>
{
    public StringInputProvider()
    {
    }

    public Task<string?> PromptAsync(string message)
    {
        return this.PromptAsync(message, string.Empty);
    }

    public Task<string?> PromptAsync(string message, string defaultValue)
    {
        var tcs = new TaskCompletionSource<string?>();
        var dialog = new StringInputDialog(message, defaultValue);

        Dispatcher.CurrentDispatcher.Invoke(() =>
        {
            if (dialog.ShowDialog() == true)
            {
                tcs.SetResult(dialog.UserInput);
            }
            else
            {
                tcs.SetResult(null);
            }
        });

        return tcs.Task;
    }
}
