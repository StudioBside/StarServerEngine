namespace Du.Core.Bases;

using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Du.Core.Interfaces;

public abstract class VmPageBase : ObservableObject, INavigationAware
{
    private string title = string.Empty;

    protected VmPageBase()
    {
    }

    public string Title
    {
        get => this.title;
        set => this.SetProperty(ref this.title, value);
    }

    public virtual Task<bool> CanExitPage()
    {
        return Task.FromResult(true);
    }

    public virtual void OnNavigating(object sender, Uri uri, CancelEventArgs args)
    {
    }

    public virtual void OnNavigated(object sender, Uri uri)
    {
    }
}
