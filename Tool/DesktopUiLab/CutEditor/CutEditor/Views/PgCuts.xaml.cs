namespace CutEditor.Views;

using System;
using System.Windows.Controls;
using CutEditor.ViewModel;
using Du.Core.Util;
using Wpf.Ui;

public sealed partial class PgCuts : Page
{
    public PgCuts()
    {
        this.InitializeComponent();
        this.DataContext = App.Current.Services.GetService<VmCuts>();
    }

    private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        App.Current.Services.GetService<ISnackbarService>().Show("Title", "Hello, Snackbar!", Wpf.Ui.Controls.ControlAppearance.Primary, icon: null, TimeSpan.FromSeconds(3));
    }
}
