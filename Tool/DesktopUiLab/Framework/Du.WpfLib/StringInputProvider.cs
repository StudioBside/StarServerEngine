namespace Du.WpfLib;

using System.Threading.Tasks;
using Du.Core.Interfaces;
using Wpf.Ui;

public sealed class StringInputProvider(IContentDialogService contentDialogService) : IUserInputProvider<string>
{
    public Task<string?> PromptAsync(string title, string message)
    {
        return this.PromptAsync(title, message, string.Empty);
    }

    public async Task<string?> PromptAsync(string title, string message, string defaultValue)
    {
        var dialog = new StringInputDialog(contentDialogService.GetDialogHost(), title, message, defaultValue);

        _ = await dialog.ShowAsync();
        return dialog.UserInput;
    }
}
