namespace CutEditor.Views;

using System.Windows.Controls;
using CutEditor.Model;
using CutEditor.ViewModel;
using Microsoft.Extensions.DependencyInjection;

public sealed partial class PgCuts : Page
{
    public PgCuts(VmCutsParam param)
    {
        this.InitializeComponent();
        this.DataContext = App.Current.Services.GetRequiredService<VmCuts.Factory>().Create(param);
    }

    public PgCuts(CutScene cutscene) : this(new VmCutsParam { CutScene = cutscene })
    {
    }

    public PgCuts(VmCut vmCut) : this(new VmCutsParam { NewFileName = vmCut.CutsceneName, CutUid = vmCut.Cut.Uid })
    {
    }
}
