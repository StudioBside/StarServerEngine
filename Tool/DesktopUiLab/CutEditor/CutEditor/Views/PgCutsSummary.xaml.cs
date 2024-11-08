namespace CutEditor.Views;

using System.Windows.Controls;
using CutEditor.ViewModel;
using Du.Core.Util;
using Microsoft.Extensions.DependencyInjection;

public sealed partial class PgCutsSummary : Page
{
    public PgCutsSummary()
    {
        this.InitializeComponent();
        this.DataContext = App.Current.Services.GetRequiredService<VmCutsSummary>();
    }
}
