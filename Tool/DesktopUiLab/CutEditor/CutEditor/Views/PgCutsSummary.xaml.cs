namespace CutEditor.Views;

using System.Windows.Controls;
using CutEditor.ViewModel;
using Microsoft.Extensions.DependencyInjection;

public sealed partial class PgCutsSummary : Page
{
    public PgCutsSummary(VmCutsSummary.CreateParam param)
    {
        this.InitializeComponent();
        this.DataContext = App.Current.Services.GetRequiredService<VmCutsSummary.Factory>().Create(param);
    }
}
