namespace CutEditor.ViewModel;

using System.Collections.ObjectModel;
using System.ComponentModel;
using Cs.Core.Util;
using Cs.Logging;
using CutEditor.Model;
using CutEditor.Model.Interfaces;
using CutEditor.ViewModel.Detail;
using Du.Core.Bases;
using Microsoft.Extensions.Configuration;

public sealed class VmCuts : VmPageBase
{
    private static CutScene lastCutSceneHistory = null!;

    private readonly CutScene cutscene;
    private readonly ObservableCollection<VmCut> cuts = new();
    private readonly string fullFilePath;
    private readonly TempUidGenerator uidGenerator = new();

    public VmCuts(VmHome vmHome, IConfiguration config, IServiceProvider services)
    {
        this.cutscene = vmHome.SelectedCutScene ?? lastCutSceneHistory;
        this.Title = $"{this.cutscene.Title} - {this.cutscene.FileName}";

        lastCutSceneHistory = this.cutscene;

        var path = config["CutFilesPath"] ?? throw new Exception("CutFilesPath is not set in the configuration file.");
        this.fullFilePath = Path.Combine(path, $"CLIENT_{this.cutscene.FileName}.exported");

        if (File.Exists(this.fullFilePath) == false)
        {
            Log.Debug($"cutscene file not found: {this.fullFilePath}");
            return;
        }

        var json = JsonUtil.Load(this.fullFilePath);
        json.GetArray("Data", this.cuts, (e, i) =>
        {
            var cut = new Cut(e, this.uidGenerator.Generate());
            return new VmCut(cut, services);
        });

        Log.Info($"cutscene loading finished. {this.cuts.Count} cuts loaded.");
    }

    public IList<VmCut> Cuts => this.cuts;

    //// --------------------------------------------------------------------------------------------

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        //switch (e.PropertyName)
        //{
        //    case nameof(this.SearchKeyword):
        //        this.filteredList.Refresh(this.searchKeyword);
        //        break;

        //    case nameof(this.SelectedCutScene):
        //        this.StartEditCommand.NotifyCanExecuteChanged();
        //        break;
        //}
    }
}