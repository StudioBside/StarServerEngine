namespace Binder.ViewModels;

using System.Windows.Input;
using System.Windows.Navigation;
using Binder.Models;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Du.Core.Bases;
using Du.Core.Models;

public sealed class VmSingleBind : VmPageBase
{
    private string message = string.Empty;
    private BindFile bindFile = null!;

    public VmSingleBind()
    {
        this.Title = "Customer";
        this.BackCommand = new RelayCommand(this.OnBack);
    }

    public ICommand BackCommand { get; set; }
    public string Message
    {
        get => this.message;
        set => this.SetProperty(ref this.message, value);
    }

    public BindFile BindFile
    {
        get => this.bindFile;
        set => this.SetProperty(ref this.bindFile, value);
    }

    public override void OnNavigated(object sender, NavigationEventArgs navigatedEventArgs)
    {
        this.Message = "Navigated";
    }

    private void OnBack()
    {
        WeakReferenceMessenger.Default.Send(new NavigationMessage("GoBack"));
    }
}
