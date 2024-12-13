namespace CutEditor.ViewModel;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Cs.Logging;
using CutEditor.Model;
using CutEditor.Model.Detail;
using CutEditor.Model.Interfaces;
using CutEditor.ViewModel.Detail;
using Du.Core.Bases;
using Du.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

public sealed class VmL10n : VmPageBase
{
    private readonly Dictionary<long/*uid*/, Cut> originCuts = new();
    private readonly ObservableCollection<CutOutputExcelFormat> importedCuts = new();
    private readonly ObservableCollection<L10nMapping> mappings = new();
    private readonly ObservableCollection<string> logMessages = new();
    private readonly IServiceProvider services;
    private string? importFilePath;

    public VmL10n(IServiceProvider services, CreateParam param)
    {
        this.services = services;

        this.Name = param.Name;
        this.Title = this.Name;
        this.LoadFileCommand = new RelayCommand(this.OnLoadFile);

        this.TextFileName = CutFileIo.GetTextFileName(this.Name);
        var cutList = CutFileIo.LoadCutData(this.Name);
        if (cutList.Count == 0)
        {
            Log.Warn($"{this.Name} 파일에 컷이 없습니다.");
            return;
        }

        var uidGenerator = new CutUidGenerator(cutList);
        foreach (var cut in cutList.Where(e => e.Choices.Any()))
        {
            var choiceUidGenerator = new ChoiceUidGenerator(cut.Uid, cut.Choices);
        }

        this.originCuts = cutList.ToDictionary(e => e.Uid);
    }

    public string Name { get; }
    public string TextFileName { get; }
    public ICommand LoadFileCommand { get; }
    public IEnumerable<Cut> OriginCuts => this.originCuts.Values;
    public IList<string> LogMessages => this.logMessages;
    public string? ImportFileName => Path.GetFileName(this.importFilePath);
    public string? ImportFilePath
    {
        get => this.importFilePath;
        set => this.SetProperty(ref this.importFilePath, value);
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

    private void OnLoadFile()
    {
        var picker = this.services.GetRequiredService<IFilePicker>();
        this.ImportFilePath = picker.PickFile(Environment.CurrentDirectory, "엑셀 파일 (*.xlsx)|*.xlsx");
        if (this.importFilePath is null)
        {
            return;
        }

        this.importedCuts.Clear();
        this.mappings.Clear();
        this.logMessages.Clear();

        var reader = this.services.GetRequiredService<IExcelFileReader>();
        if (reader.Read(this.importFilePath, this.importedCuts) == false)
        {
            Log.Error($"엑셀 파일 읽기에 실패했습니다. fileName:{this.importFilePath}");
            return;
        }

        // -------------------- mapping data --------------------
        var mappings = new Dictionary<long, L10nMapping>();
        foreach (var imported in this.importedCuts)
        {
            if (long.TryParse(imported.Uid, out var uid) == false)
            {
                // check if uid is 'x-y' format. using regex
                if (Regex.IsMatch(imported.Uid, @"^\d+-\d+$") == false)
                {
                    this.WriteLog($"잘못된 UID 형식입니다. {imported.Uid}");
                    continue;
                }

                // extract long type x from 'x-y' string
                uid = long.Parse(imported.Uid.Split('-')[0]);
            }

            if (this.originCuts.TryGetValue(uid, out var origin) == false)
            {
                this.WriteLog($"원본 컷을 찾을 수 없습니다. uid:{uid}");
                continue;
            }

            if (mappings.TryGetValue(uid, out var mapping) == false)
            {
                mapping = new L10nMapping(origin);
                mappings.Add(uid, mapping);
                this.mappings.Add(mapping);
            }

            mapping.Add(imported);
        }
    }

    private void WriteLog(string message)
    {
        var formatted = $"[{DateTime.Now:HH:mm:ss}] {message}";
        this.logMessages.Add(formatted);
    }

    public sealed record CreateParam(string Name);

    public sealed class Factory(IServiceProvider services)
    {
        public VmPageBase Create(CreateParam param)
        {
            return new VmL10n(services, param);
        }
    }
}