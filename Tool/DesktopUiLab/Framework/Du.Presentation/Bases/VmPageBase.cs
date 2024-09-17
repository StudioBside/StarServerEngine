namespace Du.Presentation.Bases;

using System.Windows.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using Du.Presentation.Interfaces;

public abstract class VmPageBase : ObservableObject, INavigationAware
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
