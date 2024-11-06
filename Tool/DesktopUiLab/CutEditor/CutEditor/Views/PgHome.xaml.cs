namespace CutEditor.Views;

using System.Windows.Controls;
using CutEditor.ViewModel;
using Microsoft.Extensions.DependencyInjection;

public sealed partial class PgHome : Page
{
    public PgHome()
    {
        this.InitializeComponent();
        this.DataContext = App.Current.Services.GetRequiredService<VmHome>();
    }
}
