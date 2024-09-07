namespace Binder.Bases;

using CommunityToolkit.Mvvm.ComponentModel;

public abstract class ViewModelBase : ObservableObject
{
    private string title = string.Empty;

    public string Title
    {
        get => this.title;
        set => this.SetProperty(ref this.title, value);
    }
}
