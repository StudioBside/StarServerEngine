namespace Du.WpfLib;

using System.Windows.Threading;
using Du.Core.Interfaces;
using Wpf.Ui;
using Wpf.Ui.Extensions;

public sealed class PopupMessageNotifier(IContentDialogService contentDialogService) : IPopupMessageNotifier
{
    public void NotifyError(string message)
    {
        Dispatcher.CurrentDispatcher.Invoke(async () =>
        {
            _ = await contentDialogService.ShowAlertAsync("오류", message, "확인");
        });
    }

    public void Notify(string title, string message)
    {
        Dispatcher.CurrentDispatcher.Invoke(async () =>
        {
            _ = await contentDialogService.ShowAlertAsync(title, message, "확인");
        });
    }
}
