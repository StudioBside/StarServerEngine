namespace Du.Core.Bases;

using CommunityToolkit.Mvvm.ComponentModel;
using Du.Core.Interfaces;

public abstract class VmPageBase : ObservableObject, INavigationAware
{
    private string title = string.Empty;

    public string Title
    {
        get => this.title;
        set => this.SetProperty(ref this.title, value);
    }

    public virtual void OnNavigating(object sender, Uri uri)
    {
    }

    public virtual void OnNavigated(object sender, Uri uri)
    {
    }
}
