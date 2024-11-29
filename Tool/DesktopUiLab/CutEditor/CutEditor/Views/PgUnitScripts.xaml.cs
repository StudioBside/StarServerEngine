namespace CutEditor.Views;

using System.Windows.Controls;
using CutEditor.ViewModel;
using Microsoft.Extensions.DependencyInjection;

public sealed partial class PgUnitScripts : Page
{
    public PgUnitScripts()
    {
        this.InitializeComponent();
        this.DataContext = App.Current.Services.GetRequiredService<VmUnitScripts>();
    }
}
