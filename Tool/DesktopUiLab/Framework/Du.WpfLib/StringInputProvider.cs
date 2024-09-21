namespace Du.WpfLib;

using System.Threading.Tasks;
using System.Windows.Threading;
using Du.Core.Interfaces;
using Wpf.Ui;

public sealed class StringInputProvider(IContentDialogService contentDialogService) : IUserInputProvider<string>
{
    public Task<string?> PromptAsync(string message)
    {
        return this.PromptAsync(message, string.Empty);
    }

    public async Task<string?> PromptAsync(string message, string defaultValue)
    {
        var dialog = new StringInputDialog(contentDialogService.GetDialogHost(), message, defaultValue);

        _ = await dialog.ShowAsync();
        return dialog.UserInput;
    }
}
