namespace Du.WpfLib;

using System.Windows.Controls;
using System.Windows.Threading;
using Du.Core.Interfaces;
using Wpf.Ui;
using Wpf.Ui.Controls;

public sealed class WaitingNotifierDialog(IContentDialogService contentDialogService) : IUserWaitingNotifier
{
    private ContentDialog? dialog;

    public Task<IDisposable> StartWait(string message)
    {
        var tcs = new TaskCompletionSource<IDisposable>();

        Dispatcher.CurrentDispatcher.Invoke(() =>
        {
            this.dialog = new ContentDialog();
            this.dialog.Opened += (sender, e) => tcs.SetResult(new Closer(this));

            this.dialog.SetCurrentValue(ContentDialog.TitleProperty, "Waiting");
            this.dialog.SetCurrentValue(ContentControl.ContentProperty, new ProgressRing { IsIndeterminate = true });
            this.dialog.IsFooterVisible = false;

            contentDialogService.ShowAsync(this.dialog, cancellationToken: default);
        });

        return tcs.Task;
    }

    public void StopWait()
    {
        if (this.dialog is null)
        {
            return;
        }

        this.dialog.Hide();
        this.dialog = null;
    }

    private sealed class Closer(WaitingNotifierDialog notifier) : IDisposable
    {
        public void Dispose() => notifier.StopWait();
    }
}