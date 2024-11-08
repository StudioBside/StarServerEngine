namespace CutEditor.ViewModel;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Cs.Core.Util;
using Cs.Logging;
using CutEditor.Model;
using CutEditor.ViewModel.Detail;
using Du.Core.Bases;
using Du.Core.Interfaces;
using Du.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public sealed class VmCutsSummary : VmPageBase
{
    private readonly ObservableCollection<VmCut> cuts = new();
    private readonly ObservableCollection<VmCut> selectedCuts = new();
    private readonly string textFilePath;
    private readonly string name;
    private readonly CutUidGenerator uidGenerator = new();
    private readonly IServiceProvider services;
    private readonly IServiceScope serviceScope;

    public VmCutsSummary(IConfiguration config, IServiceProvider services)
    {
        this.serviceScope = services.CreateScope();
        this.services = services;
        this.GoToListCommand = new RelayCommand(this.OnGoToList);
        this.EditCommand = new AsyncRelayCommand(this.OnEdit);
        this.OpenFileCommand = new RelayCommand(this.OnOpenFile);
        this.CopyFileNameCommand = new RelayCommand(this.OnCopyFileName);

        if (VmGlobalState.Instance.VmCutsCreateParam is null)
        {
            throw new Exception($"VmCuts.CreateParam is not set in the GlobalState.");
        }

        var param = VmGlobalState.Instance.VmCutsCreateParam;
        this.textFilePath = config["CutTextFilePath"] ?? throw new Exception("CutTextFilePath is not set in the configuration file.");

        if (param.CutScene is null)
        {
            this.name = param.NewFileName ?? throw new Exception("invalid createParam. newFileName is empty.");
            this.Title = $"새로운 파일 생성 - {this.name}";
        }
        else
        {
            this.name = param.CutScene.FileName;
            this.Title = $"{param.CutScene.Title} - {this.name}";
        }

        var textFileName = this.GetTextFileName();
        if (File.Exists(textFileName) == false)
        {
            Log.Debug($"cutscene file not found: {textFileName}");
            return;
        }

        var json = JsonUtil.Load(textFileName);
        json.GetArray("Data", this.cuts, (e, i) =>
        {
            var cut = new Cut(e);
            return new VmCut(cut, this.services);
        });

        this.uidGenerator.Initialize(this.cuts);

        Log.Info($"{this.name} 파일 로딩 완료. 총 컷의 개수:{this.cuts.Count}");
    }

    public IList<VmCut> Cuts => this.cuts;
    public IList<VmCut> SelectedCuts => this.selectedCuts;
    public ICommand GoToListCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand OpenFileCommand { get; }
    public ICommand CopyFileNameCommand { get; }

    private string DebugName => $"[{this.name}]";

    public override void OnNavigating(object sender, Uri uri)
    {
        // 다른 페이지로의 네이게이션이 시작될 때 (= 지금 페이지가 닫힐 때)
        Log.Debug($"{this.DebugName} OnNavigating: {uri}");

        this.serviceScope.Dispose();
    }

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

    private void OnGoToList()
    {
        WeakReferenceMessenger.Default.Send(new NavigationMessage("Views/PgHome.xaml"));
    }

    private async Task OnEdit()
    {
        await Task.Delay(0);
        WeakReferenceMessenger.Default.Send(new NavigationMessage("Views/PgCuts.xaml"));
    }

    private string GetTextFileName() => Path.Combine(this.textFilePath, $"CLIENT_{this.name}.exported");

    private void OnOpenFile()
    {
        var fileName = this.GetTextFileName();
        if (File.Exists(fileName) == false)
        {
            Log.Error($"{this.DebugName} 파일이 존재하지 않습니다.\n{fileName}");
            return;
        }

        var fullPath = Path.GetFullPath(fileName);
        Process.Start(new ProcessStartInfo(fullPath) { UseShellExecute = true });
    }

    private void OnCopyFileName()
    {
        var clipboardWriter = this.services.GetRequiredService<IClipboardWriter>();
        clipboardWriter.SetText(this.name);

        Log.Info($"{this.DebugName} 파일명을 클립보드에 복사했습니다.");
    }
}