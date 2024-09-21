namespace Du.WpfLib;

using System.Windows.Threading;
using Du.Core.Interfaces;
using Wpf.Ui;
using Wpf.Ui.Extensions;

public sealed class ErrorNotifierDialog(IContentDialogService contentDialogService) : IUserErrorNotifier
{
    public void NotifyError(string message)
    {
        Dispatcher.CurrentDispatcher.Invoke(async () =>
        {
            _ = await contentDialogService.ShowAlertAsync("오류", message, "확인");
        });
    }
}
