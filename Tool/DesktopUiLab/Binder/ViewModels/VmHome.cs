namespace Binder.ViewModels;

using System.Windows.Navigation;
using Binder.Bases;

public sealed class VmHome : ViewModelBase
{
    private string message = string.Empty;

    public VmHome()
    {
        this.Title = "Home";
    }

    public static int Count { get; set; }
    public string Message
    {
        get => this.message;
        set => this.SetProperty(ref this.message, value);
    }

    public override void OnNavigated(object sender, NavigationEventArgs navigatedEventArgs)
    {
        Count++;
        this.Message = $"{Count} Navigated";
    }
}
