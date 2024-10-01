namespace CutEditor.ViewModel;

using System.Collections.ObjectModel;
using System.ComponentModel;
using Cs.Core.Util;
using Cs.Logging;
using CutEditor.Model;
using Du.Core.Bases;
using Microsoft.Extensions.Configuration;

public sealed class VmCuts : VmPageBase
{
    private static CutScene lastCutSceneHistory = null!;

    private readonly CutScene cutscene;
    private readonly ObservableCollection<Cut> cuts = new();
    private readonly string fullFilePath;

    public VmCuts(VmHome vmHome, IConfiguration config)
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
        json.GetArray("Data", this.cuts, (e, i) => new Cut(e));

        Log.Info($"cutscene loading finished. {this.cuts.Count} cuts loaded.");
    }

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