namespace CutEditor.Views;

using System.Windows.Controls;
using CutEditor.ViewModel;
using Microsoft.Extensions.DependencyInjection;

public sealed partial class PgUnits : Page
{
    public PgUnits()
    {
        this.InitializeComponent();
        this.DataContext = App.Current.Services.GetRequiredService<VmUnits>();
    }
}
