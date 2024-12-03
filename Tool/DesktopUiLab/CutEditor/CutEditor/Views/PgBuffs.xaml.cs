namespace CutEditor.Views;

using System.Windows.Controls;
using CutEditor.ViewModel;
using Microsoft.Extensions.DependencyInjection;

public sealed partial class PgBuffs : Page
{
    public PgBuffs()
    {
        this.InitializeComponent();
        this.DataContext = App.Current.Services.GetRequiredService<VmBuffs>();
    }
}
