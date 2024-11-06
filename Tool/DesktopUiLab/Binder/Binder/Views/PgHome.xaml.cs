namespace Binder.Views;

using System.Windows.Controls;
using Binder.ViewModel;
using Microsoft.Extensions.DependencyInjection;

public partial class PgHome : Page
{
    public PgHome()
    {
        this.InitializeComponent();
        this.DataContext = App.Current.Services.GetRequiredService<VmHome>();
    }
}
