namespace CutEditor.Views;

using System.Windows.Controls;
using CutEditor.ViewModel;
using Du.Core.Util;

public sealed partial class PgHome : Page
{
    public PgHome()
    {
        this.InitializeComponent();
        this.DataContext = App.Current.Services.GetServiceNotNull<VmHome>();
    }
}
