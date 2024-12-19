namespace CutEditor.Views;

using System.Windows.Controls;
using CutEditor.ViewModel;
using Microsoft.Extensions.DependencyInjection;

public sealed partial class PgSkills : Page
{
    public PgSkills()
    {
        this.InitializeComponent();
        this.DataContext = App.Current.Services.GetRequiredService<VmSkills>();
    }
}
