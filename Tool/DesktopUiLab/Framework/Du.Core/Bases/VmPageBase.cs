namespace Du.Core.Bases;

using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Du.Core.Interfaces;
using Du.Core.Models;

public abstract class VmPageBase : ObservableObject, INavigationAware
{
    private string title = string.Empty;

    protected VmPageBase()
    {
        this.GoToCommand = new RelayCommand<string>(this.OnGoTo);
    }

    public string Title
    {
        get => this.title;
        set => this.SetProperty(ref this.title, value);
    }

    public ICommand GoToCommand { get; }

    public virtual void OnNavigating(object sender, Uri uri)
    {
    }

    public virtual void OnNavigated(object sender, Uri uri)
    {
    }

    //// ------------------------------------------------------------------------------------

    private void OnGoTo(string? obj)
    {
        if (string.IsNullOrEmpty(obj))
        {
            return;
        }

        WeakReferenceMessenger.Default.Send(new NavigationMessage(obj));
    }
}
