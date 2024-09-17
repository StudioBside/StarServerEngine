namespace Binder.Views;

using System.Windows.Controls;
using Binder.ViewModel;
using Du.Core.Util;

public partial class PgHome : Page
{
    public PgHome()
    {
        this.InitializeComponent();
        this.DataContext = App.Current.Services.GetService<VmHome>();
    }
}
