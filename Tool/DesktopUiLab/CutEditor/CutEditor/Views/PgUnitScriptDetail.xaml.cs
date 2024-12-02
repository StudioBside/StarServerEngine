namespace CutEditor.Views;

using System.Windows.Controls;
using CutEditor.ViewModel;
using ICSharpCode.AvalonEdit.Highlighting;
using Shared.Templet.UnitScripts;

public sealed partial class PgUnitScriptDetail : Page
{
    public PgUnitScriptDetail(UnitScript script)
    {
        this.InitializeComponent();
        this.DataContext = new VmUnitScriptDetail(script);

        this.JsonTextEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("JavaScript");
        this.JsonTextEditor.Text = script.FullText;
    }
}
