namespace Du.WpfLib;

using System.Windows.Threading;
using Cs.Logging;
using Du.Core.Interfaces;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

public sealed class ErrorNotifierDialog(IContentDialogService contentDialogService) : IUserErrorNotifier
{
    public void NotifyError(string message)
    {
        Dispatcher.CurrentDispatcher.Invoke(async () =>
        {
            ContentDialogResult result = await contentDialogService.ShowAlertAsync("Error", message, "OK");
            Log.Debug(result.ToString());
        });
    }
}
