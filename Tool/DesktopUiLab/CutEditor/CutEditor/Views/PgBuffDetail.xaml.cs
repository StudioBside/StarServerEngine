namespace CutEditor.Views;

using System.Windows.Controls;
using CutEditor.ViewModel;
using Shared.Templet.TempletTypes;

public sealed partial class PgBuffDetail : Page
{
    public PgBuffDetail(BuffTemplet buffTemplet)
    {
        this.InitializeComponent();
        this.DataContext = new VmBuffDetail(buffTemplet);
    }
}
