namespace CutEditor.ViewModel;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CutEditor.ViewModel.UndoCommands;

public class VmCutPaster : ObservableObject
{
    private readonly VmCuts vmCuts;
    private readonly ObservableCollection<VmCut> reserved = [];

    public VmCutPaster(VmCuts vmCuts)
    {
        this.vmCuts = vmCuts;
        this.CutCommand = new RelayCommand(this.OnCut, () => this.vmCuts.SelectedCuts.Count > 0);
        this.PasteToUpsideCommand = new RelayCommand(this.OnPasteToUpside, () => this.reserved.Count > 0);
        this.PasteToDownsideCommand = new RelayCommand(this.OnPasteToDownside, () => this.reserved.Count > 0);
        this.CancelCommand = new RelayCommand(this.OnCancel);

        this.reserved.CollectionChanged += this.Reserved_CollectionChanged;
        this.vmCuts.PropertyChanged += this.VmCuts_PropertyChanged;
    }

    public IList<VmCut> Reserved => this.reserved;
    public bool HasReserved => this.reserved.Count > 0;
    public IRelayCommand CutCommand { get; }
    public IRelayCommand PasteToUpsideCommand { get; }
    public IRelayCommand PasteToDownsideCommand { get; }
    public ICommand CancelCommand { get; }

    public void ClearReserved()
    {
        this.reserved.Clear();
    }

    public void SetReserved(IEnumerable<VmCut> cuts)
    {
        this.reserved.Clear();
        foreach (var cut in cuts)
        {
            this.reserved.Add(cut);
        }
    }

    //// --------------------------------------------------------------------------------------------

    private void VmCuts_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(this.vmCuts.SelectedCuts):
                this.CutCommand.NotifyCanExecuteChanged();
                break;
        }
    }

    private void Reserved_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        this.PasteToUpsideCommand.NotifyCanExecuteChanged();
        this.PasteToDownsideCommand.NotifyCanExecuteChanged();
        this.OnPropertyChanged(nameof(this.HasReserved));
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

    private void OnPasteToUpside()
    {
        var command = PasteCut.Create(this.vmCuts, PasteCut.PasteDirection.Upside);
        if (command is null)
        {
            return;
        }

        command.Redo();
        this.vmCuts.UndoController.Add(command);
    }
    
    private void OnPasteToDownside()
    {
        var command = PasteCut.Create(this.vmCuts, PasteCut.PasteDirection.Downside);
        if (command is null)
        {
            return;
        }

        command.Redo();
        this.vmCuts.UndoController.Add(command);
    }

    private void OnCancel()
    {
        this.reserved.Clear();
    }
}
