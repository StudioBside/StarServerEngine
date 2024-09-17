namespace Binder.ViewModel;

using System.Windows.Input;
using Binder.Model;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Du.Core.Bases;
using Du.Core.Models;

public sealed class VmExtract : VmPageBase
{
    private readonly Extract extract;

    public VmExtract(VmHome vmHome)
    {
        this.Title = "Customer";
        this.BackCommand = new RelayCommand(this.OnBack);
        this.extract = vmHome.SelectedExtract 
            ?? throw new ArgumentNullException(nameof(vmHome.SelectedExtract));
    }

    public Extract Extract => this.extract;
    public ICommand BackCommand { get; set; }

    private void OnBack()
    {
        WeakReferenceMessenger.Default.Send(new NavigationMessage("GoBack"));
    }
}
