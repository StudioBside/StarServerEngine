namespace Du.WpfLib;

using System.Threading.Tasks;
using System.Windows;
using Du.Core.Interfaces;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

public sealed class DialogProvider(IContentDialogService contentDialogService) : IDialogProvider
{
    public IDialogProvider.ResultType Show(string title, string message)
    {
        // note: messageBox 위치를 윈도우 중앙으로 하고자 window를 입력했지만 제대로 동작하지 않습니다.
        Window window = Application.Current.MainWindow;

        var result = System.Windows.MessageBox.Show(window, message, title, System.Windows.MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        return result switch
        {
            System.Windows.MessageBoxResult.Yes => IDialogProvider.ResultType.Primary,
            System.Windows.MessageBoxResult.No => IDialogProvider.ResultType.Secondary,
            _ => IDialogProvider.ResultType.None,
        };
    }

    public async Task<IDialogProvider.ResultType> ShowAsync(string title, string message, string btnText1, string btnText2, string btnText3)
    {
        var options = new SimpleContentDialogCreateOptions
        {
            Title = title,
            Content = message,
            PrimaryButtonText = btnText1,
            SecondaryButtonText = btnText2,
            CloseButtonText = btnText3,
        };
        var result = await contentDialogService.ShowSimpleDialogAsync(options);
        return result switch
        {
            ContentDialogResult.Primary => IDialogProvider.ResultType.Primary,
            ContentDialogResult.Secondary => IDialogProvider.ResultType.Secondary,
            _ => IDialogProvider.ResultType.None,
        };
    }
}
