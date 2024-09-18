namespace Binder.ViewModel;

using System.Windows.Input;
using Binder.Model;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Cs.Logging;
using Du.Core.Bases;
using Du.Core.Interfaces;
using Du.Core.Models;

public sealed class VmExtract : VmPageBase
{
    private readonly Extract extract;
    private readonly ICollectionEditor collectionEditor;
    private readonly IUserWaitingNotifier waitingNotifier;

    public VmExtract(VmHome vmHome, ICollectionEditor collectionEditor, IUserWaitingNotifier waitingNotifier)
    {
        this.collectionEditor = collectionEditor;
        this.waitingNotifier = waitingNotifier;
        this.Title = "Customer";
        this.BackCommand = new RelayCommand(this.OnBack);
        this.EditColumnsCommand = new AsyncRelayCommand(this.OnEditColumns);
        this.extract = vmHome.SelectedExtract 
            ?? throw new ArgumentNullException(nameof(vmHome.SelectedExtract));
    }

    public Extract Extract => this.extract;
    public ICommand BackCommand { get; }
    public ICommand EditColumnsCommand { get; }

    //// ------------------------------------------------------------------------------------

    private void OnBack()
    {
        WeakReferenceMessenger.Default.Send(new NavigationMessage("GoBack"));
    }

    private async Task OnEditColumns()
    {
        using var waiting = await this.waitingNotifier.StartWait("Edit collection");
        var result = await this.collectionEditor.Edit(this.extract.BindRoot.Columns);
    }
}
