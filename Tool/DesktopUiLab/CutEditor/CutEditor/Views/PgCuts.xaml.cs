namespace CutEditor.Views;

using System.Windows.Controls;
using CutEditor.Model;
using CutEditor.Model.CutSearch;
using CutEditor.ViewModel;
using Microsoft.Extensions.DependencyInjection;

public sealed partial class PgCuts : Page
{
    public PgCuts(VmCuts.CreateParam param)
    {
        this.InitializeComponent();
        this.DataContext = App.Current.Services.GetRequiredService<VmCuts.Factory>().Create(param);
    }

    public PgCuts(CutScene cutscene) : this(new VmCuts.CreateParam(cutscene.FileName, CutUid: 0))
    {
    }

    public PgCuts(VmCutSummary summary) : this(new VmCuts.CreateParam(summary.CutsceneName, summary.Cut.Uid))
    {
    }

    public PgCuts(CutSearchResult search) : this(new VmCuts.CreateParam(search.CutScene.FileName, search.Cut.Uid))
    {
    }
}
