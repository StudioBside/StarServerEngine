namespace CutEditor.Views;

using System.Windows.Controls;
using CutEditor.ViewModel;
using Microsoft.Extensions.DependencyInjection;

public sealed partial class PgStrings : Page
{
    public PgStrings()
    {
        this.InitializeComponent();
        this.DataContext = App.Current.Services.GetRequiredService<VmStrings>();
    }
}
