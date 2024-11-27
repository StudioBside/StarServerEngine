namespace CutEditor.Views;

using System.Windows.Controls;
using CutEditor.ViewModel;
using Microsoft.Extensions.DependencyInjection;

public sealed partial class PgErrors : Page
{
    public PgErrors()
    {
        this.InitializeComponent();
        this.DataContext = App.Current.Services.GetRequiredService<VmErrors>();
    }
}
