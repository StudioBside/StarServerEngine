namespace CutEditor.Views;

using System.Windows.Controls;
using CutEditor.ViewModel;
using Microsoft.Extensions.DependencyInjection;

public sealed partial class PgCutSearch : Page
{
    public PgCutSearch()
    {
        this.InitializeComponent();
        this.DataContext = App.Current.Services.GetRequiredService<VmCutSearch>();
    }
}
