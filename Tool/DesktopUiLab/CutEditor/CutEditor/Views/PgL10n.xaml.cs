namespace CutEditor.Views;

using System.Windows.Controls;
using CutEditor.ViewModel;
using Microsoft.Extensions.DependencyInjection;

public sealed partial class PgL10n : Page
{
    public PgL10n(VmL10n.CreateParam param)
    {
        this.InitializeComponent();
        this.DataContext = App.Current.Services.GetRequiredService<VmL10n.Factory>().Create(param);
    }
}
