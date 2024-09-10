namespace Binder.ViewModels;

using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Du.Core.Bases;
using Du.Core.Models;

public sealed class VmMain : VmPagelBase
{
    private string? navigationSource = string.Empty;

    public VmMain()
    {
        this.Title = "Main View";
        this.NavigationSource = "Views/HomePage.xaml";
        this.NavigateCommand = new RelayCommand<string?>(this.OnNavigate);
    
        WeakReferenceMessenger.Default.Register<NavigationMessage>(this, this.OnNavigationMessage);
    }

    public string? NavigationSource
    {
        get => this.navigationSource;
        set => this.SetProperty(ref this.navigationSource, value);
    }

    public ICommand NavigateCommand { get; }

    //// --------------------------------------------------------------------------------------------

    private void OnNavigate(string? pageUri)
    {
        // case 1. Xaml에서 ICommand로 네비게이션 할 때 
        this.NavigationSource = pageUri;
    }

    private void OnNavigationMessage(object recipient, NavigationMessage message)
    {
        // case 2. 다른 ViewModel(code) 에서 메시지로 네비게이션 할 때
        this.NavigationSource = message.Value;
    }
}
