namespace CutEditor.Views;

using System.Windows.Controls;
using CutEditor.ViewModel;
using ICSharpCode.AvalonEdit.Highlighting;
using Shared.Templet.TempletTypes;

public sealed partial class PgUnitDetail : Page
{
    public PgUnitDetail(Unit unitTemplet)
    {
        this.InitializeComponent();
        this.DataContext = new VmUnitDetail(unitTemplet);

        if (unitTemplet.Script is not null)
        {
            this.JsonTextEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("JavaScript");
            this.JsonTextEditor.Text = unitTemplet.Script.FullText;
        }
    }
}
