namespace CutEditor.ViewModel;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public class VmCutPaster : ObservableObject
{
    private readonly VmCuts vmCuts;
    private readonly ObservableCollection<VmCut> reserved = [];

    public VmCutPaster(VmCuts vmCuts)
    {
        this.vmCuts = vmCuts;
        this.CutCommand = new RelayCommand(this.OnCut, () => this.vmCuts.SelectedCuts.Count > 0);
        this.PasteToSelectedCommand = new RelayCommand(this.OnPasteToSelected, () => this.reserved.Count > 0);
        this.PasteToNextCommand = new RelayCommand(this.OnPasteToNext, () => this.reserved.Count > 0);
        this.CancelCommand = new RelayCommand(this.OnCancel);

        this.reserved.CollectionChanged += this.Reserved_CollectionChanged;
        this.vmCuts.PropertyChanged += this.VmCuts_PropertyChanged;
    }

    public IList<VmCut> Reserved => this.reserved;
    public bool HasReserved => this.reserved.Count > 0;
    public IRelayCommand CutCommand { get; }
    public IRelayCommand PasteToSelectedCommand { get; }
    public IRelayCommand PasteToNextCommand { get; }
    public ICommand CancelCommand { get; }

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
        this.PasteToSelectedCommand.NotifyCanExecuteChanged();
        this.PasteToNextCommand.NotifyCanExecuteChanged();
        this.OnPropertyChanged(nameof(this.HasReserved));
    }

    private void OnCut()
    {
        if (this.vmCuts.SelectedCuts.Count == 0)
        {
            return;
        }

        this.reserved.Clear();
        var cuts = this.vmCuts.SelectedCuts.ToArray();
        foreach (var cut in cuts)
        {
            this.reserved.Add(cut);
            this.vmCuts.Cuts.Remove(cut);
        }
    }

    private void OnPasteToSelected()
    {
        if (this.reserved.Count == 0)
        {
            return;
        }

        if (this.vmCuts.SelectedCuts.Count == 0)
        {
            return;
        }

        var index = this.vmCuts.Cuts.IndexOf(this.vmCuts.SelectedCuts.First());
        foreach (var cut in this.reserved)
        {
            this.vmCuts.Cuts.Insert(index++, cut);
        }

        this.reserved.Clear();
    }
    
    private void OnPasteToNext()
    {
        if (this.reserved.Count == 0)
        {
            return;
        }

        if (this.vmCuts.SelectedCuts.Count == 0)
        {
            return;
        }

        var index = this.vmCuts.Cuts.IndexOf(this.vmCuts.SelectedCuts.Last()) + 1;

        foreach (var cut in this.reserved)
        {
            this.vmCuts.Cuts.Insert(index++, cut);
        }

        this.reserved.Clear();
    }

    private void OnCancel()
    {
        this.reserved.Clear();
    }
}
