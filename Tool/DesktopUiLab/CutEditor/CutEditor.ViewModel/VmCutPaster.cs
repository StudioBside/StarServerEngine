namespace CutEditor.ViewModel;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CutEditor.Model;
using CutEditor.ViewModel.UndoCommands;
using Du.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

public class VmCutPaster : ObservableObject
{
    private static readonly List<CutPreset> Presets = [];
    private readonly VmCuts vmCuts;
    private readonly ObservableCollection<VmCut> reserved = [];

    public VmCutPaster(VmCuts vmCuts)
    {
        this.vmCuts = vmCuts;
        this.CutCommand = new RelayCommand(this.OnCut, () => this.vmCuts.SelectedCuts.Count > 0);
        this.CopyCommand = new AsyncRelayCommand(this.OnCopy, () => this.vmCuts.SelectedCuts.Count > 0);
        this.PasteToUpsideCommand = new RelayCommand(this.OnPasteToUpside, () => this.HasAnyData);
        this.PasteToDownsideCommand = new RelayCommand(this.OnPasteToDownside, () => this.HasAnyData);
        this.ClearAllCommand = new RelayCommand(this.OnClearAll);

        this.reserved.CollectionChanged += this.Reserved_CollectionChanged;
        this.vmCuts.PropertyChanged += this.VmCuts_PropertyChanged;
    }

    public enum PasteDirection
    {
        Upside,
        Downside,
    }

    public IList<VmCut> Reserved => this.reserved;
    public bool HasAnyData => this.reserved.Count > 0 || Presets.Count > 0;
    public int PresetCount => Presets.Count;
    public bool HasReserved => this.reserved.Count > 0;
    public IRelayCommand CutCommand { get; }
    public IRelayCommand CopyCommand { get; }
    public IRelayCommand PasteToUpsideCommand { get; }
    public IRelayCommand PasteToDownsideCommand { get; }
    public ICommand ClearAllCommand { get; }

    public void ClearReserved()
    {
        this.reserved.Clear();
    }

    public void SetReserved(IEnumerable<VmCut> cuts)
    {
        Presets.Clear();

        this.reserved.Clear();
        foreach (var cut in cuts)
        {
            this.reserved.Add(cut);
        }
    }

    internal void MakePresets(IEnumerable<Cut> cuts)
    {
        this.reserved.Clear();

        Presets.Clear();
        foreach (var cut in cuts)
        {
            Presets.Add(new CutPreset(cut));
        }

        this.OnPropertyChanged(nameof(this.HasAnyData));
        this.OnPropertyChanged(nameof(this.PresetCount));
    }

    //// --------------------------------------------------------------------------------------------

    private void VmCuts_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(this.vmCuts.SelectedCuts):
                this.CutCommand.NotifyCanExecuteChanged();
                this.CopyCommand.NotifyCanExecuteChanged();
                break;
        }
    }

    private void Reserved_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        this.PasteToUpsideCommand.NotifyCanExecuteChanged();
        this.PasteToDownsideCommand.NotifyCanExecuteChanged();
        this.OnPropertyChanged(nameof(this.HasReserved));
        this.OnPropertyChanged(nameof(this.HasAnyData));
        this.OnPropertyChanged(nameof(this.PresetCount));
    }

    private void OnCut()
    {
        var command = ReserveCut.Create(this.vmCuts);
        if (command is null)
        {
            return;
        }

        command.Redo();
        this.vmCuts.UndoController.Add(command);
    }

    private async Task OnCopy()
    {
        if (this.vmCuts.SelectedCuts.Count == 0)
        {
            return;
        }

        if (this.vmCuts.CutPaster.HasReserved)
        {
            var prompt = this.vmCuts.Services.GetRequiredService<IUserInputProvider<bool>>();
            if (await prompt.PromptAsync("클립보드에 잘라내기 한 데이터가 있습니다.", "덮어쓰기하시겠습니까?") == false)
            {
                return;
            }

            this.vmCuts.CutPaster.ClearReserved();
        }

        this.vmCuts.CutPaster.MakePresets(this.vmCuts.SelectedCuts.Select(e => e.Cut));
    }

    private void OnPasteToUpside()
    {
        var direction = PasteDirection.Upside;
        IDormammu? command = Presets.Count > 0
            ? PasteCopiedCut.Create(this.vmCuts, Presets, direction)
            : PasteReservedCut.Create(this.vmCuts, direction);

        if (command is null)
        {
            return;
        }

        command.Redo();
        this.vmCuts.UndoController.Add(command);
    }
    
    private void OnPasteToDownside()
    {
        var direction = PasteDirection.Downside;
        IDormammu? command = Presets.Count > 0
            ? PasteCopiedCut.Create(this.vmCuts, Presets, direction)
            : PasteReservedCut.Create(this.vmCuts, direction);

        if (command is null)
        {
            return;
        }

        command.Redo();
        this.vmCuts.UndoController.Add(command);
    }

    private void OnClearAll()
    {
        this.reserved.Clear();

        if (Presets.Count > 0)
        {
            Presets.Clear();

            this.OnPropertyChanged(nameof(this.HasAnyData));
            this.OnPropertyChanged(nameof(this.PresetCount));
        }
    }
}
