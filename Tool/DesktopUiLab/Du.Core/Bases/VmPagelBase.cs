namespace Du.Core.Bases;

using System.Windows.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using Du.Core.Interfaces;

public abstract class VmPagelBase : ObservableObject, INavigationAware
{
    private string title = string.Empty;

    public string Title
    {
        get => this.title;
        set => this.SetProperty(ref this.title, value);
    }

    public virtual void OnNavigating(object sender, NavigatingCancelEventArgs navigationEventArgs)
    {
    }

    public virtual void OnNavigated(object sender, NavigationEventArgs navigatedEventArgs)
    {
    }
}
