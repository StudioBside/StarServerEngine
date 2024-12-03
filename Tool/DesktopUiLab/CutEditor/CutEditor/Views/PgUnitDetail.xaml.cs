namespace CutEditor.Views;

using System.Windows.Controls;
using CutEditor.ViewModel;
using Shared.Templet.TempletTypes;

public sealed partial class PgUnitDetail : Page
{
    public PgUnitDetail(Unit unitTemplet)
    {
        this.InitializeComponent();
        this.DataContext = new VmUnitDetail(unitTemplet);
    }
}
