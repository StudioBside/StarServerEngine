namespace Du.WpfLib;

using System.Threading.Tasks;
using Du.Core.Interfaces;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

public sealed class BooleanInputProvider(IContentDialogService contentDialogService) : IUserInputProvider<bool>
{
    public Task<bool> PromptAsync(string title, string message)
    {
        return this.PromptAsync(title, message, false);
    }

    public async Task<bool> PromptAsync(string title, string message, bool defaultValue)
    {
        var options = new SimpleContentDialogCreateOptions
        {
            Title = title,
            Content = message,
            PrimaryButtonText = "확인",
            CloseButtonText = "취소",
        };
        var result = await contentDialogService.ShowSimpleDialogAsync(options);
        return result == ContentDialogResult.Primary;
    }
}
