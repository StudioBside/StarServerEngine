namespace CutEditor;

using System.ComponentModel;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using CutEditor.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui;
using static CutEditor.Model.Messages;

public partial class MainWindow : Window
{
    private readonly double defaultMinWidth;

    public MainWindow()
    {
        this.InitializeComponent();
        this.LoadWindowState();

        var services = App.Current.Services;
        this.DataContext = services.GetRequiredService<VmMain>();

        var dialogService = services.GetRequiredService<IContentDialogService>();
        dialogService.SetDialogHost(this.RootContentDialog);

        var snackbarService = services.GetRequiredService<ISnackbarService>();
        snackbarService.SetSnackbarPresenter(this.SnackbarPresenter);

        this.defaultMinWidth = this.MinWidth;
        WeakReferenceMessenger.Default.Register<PreviewChangedMessage>(this, this.OnPreviewChanged);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);
        this.SaveWindowState();
    }
    
    private void LoadWindowState()
    {
        if (Properties.Settings.Default.Width != 0)
        {
            this.Top = Properties.Settings.Default.Top;
            this.Left = Properties.Settings.Default.Left;
            this.Width = Properties.Settings.Default.Width;
            this.Height = Properties.Settings.Default.Height;

            // 화면 밖에 있는 경우 위치 수정
            var virtualScreenWidth = SystemParameters.VirtualScreenWidth;
            var virtualScreenHeight = SystemParameters.VirtualScreenHeight;
            if (this.Top < 0 || this.Left < 0 || this.Top + this.Height > virtualScreenHeight || this.Left + this.Width > virtualScreenWidth)
            {
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            // int를 WindowState로 변환
            this.WindowState = (WindowState)Properties.Settings.Default.WindowState;

            // 프리뷰로 인한 너비 조정 이슈 : 너비는 저장값을 무시하고 항상 최소 너비로 설정.
            this.Width = this.MinWidth;
        }
    }

    private void SaveWindowState()
    {
        Properties.Settings.Default.Top = this.Top;
        Properties.Settings.Default.Left = this.Left;
        Properties.Settings.Default.Width = this.Width;
        Properties.Settings.Default.Height = this.Height;

        // WindowState를 int로 변환하여 저장
        Properties.Settings.Default.WindowState = (int)this.WindowState;

        Properties.Settings.Default.Save(); // 설정 저장
    }

    private void OnPreviewChanged(object recipient, PreviewChangedMessage message)
    {
        this.MinWidth = message.Value
            ? this.defaultMinWidth + 255
            : this.defaultMinWidth;

        this.Width = this.MinWidth;
    }
}