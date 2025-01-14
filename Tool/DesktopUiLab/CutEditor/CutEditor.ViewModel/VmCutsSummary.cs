namespace CutEditor.ViewModel;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Cs.Core.Util;
using Cs.Logging;
using CutEditor.Model;
using CutEditor.ViewModel.Detail;
using Du.Core.Bases;
using Du.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public sealed class VmCutsSummary : VmPageBase
{
    private readonly ObservableCollection<VmCutSummary> cuts = new();
    private readonly ObservableCollection<VmCutSummary> selectedCuts = new();
    private readonly CutUidGenerator uidGenerator;
    private readonly IServiceProvider services;

    public VmCutsSummary(IServiceProvider services, CreateParam param)
    {
        this.services = services;

        this.Name = param.Name;
        this.Title = this.Name;

        this.GotoEditCommand = new RelayCommand(this.OnGoToEdit);

        this.TextFileName = CutFileIo.GetTextFileName(this.Name, param.IsShorten);
        if (File.Exists(this.TextFileName) == false)
        {
            Log.Debug($"cutscene file not found: {this.TextFileName}");
            this.uidGenerator = new CutUidGenerator(Enumerable.Empty<Cut>());
            return;
        }

        var json = JsonUtil.Load(this.TextFileName);
        json.GetArray("Data", this.cuts, (e, i) =>
        {
            var cut = new Cut(e, this.Name);
            return new VmCutSummary(cut, this.Name);
        });

        this.uidGenerator = new CutUidGenerator(this.cuts.Select(e => e.Cut));

        ////Log.Info($"{this.Name} 파일 로딩 완료. 총 컷의 개수:{this.cuts.Count}");

        var removeTargets = this.cuts.Where(e => e.DataType == Enums.CutDataType.Normal && e.Cut.UnitTalk.Korean.Length == 0).ToArray();
        if (removeTargets.Length > 0)
        {
            Log.Debug($"{this.Name} 파일에서 대사가 없는 컷을 제거합니다. {removeTargets.Length}개");
            foreach (var vmCut in removeTargets)
            {
                this.cuts.Remove(vmCut);
            }
        }
    }

    public string Name { get; }
    public IList<VmCutSummary> Cuts => this.cuts;
    public IList<VmCutSummary> SelectedCuts => this.selectedCuts;
    public string TextFileName { get; }
    public ICommand GotoEditCommand { get; }

    //// --------------------------------------------------------------------------------------------

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        //switch (e.PropertyName)
        //{
        //    case nameof(this.SelectedCuts):
        //        this.DeleteCommand.NotifyCanExecuteChanged();
        //        break;
        //}
    }

    private void OnGoToEdit()
    {
        var router = this.services.GetRequiredService<IPageRouter>();
        router.Route(new VmCuts.CreateParam(this.Name, CutUid: 0));
    }

    public sealed record CreateParam(string Name)
    {
        public bool IsShorten { get; init; }
    }

    public sealed class Factory(IServiceProvider services)
    {
        public VmPageBase Create(CreateParam param)
        {
            return new VmCutsSummary(services, param);
        }
    }
}