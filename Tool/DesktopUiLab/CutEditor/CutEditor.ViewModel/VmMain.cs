namespace CutEditor.ViewModel;

using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Du.Core.Bases;
using Du.Core.Interfaces;

public sealed class VmMain : VmPageBase
{
    private bool showMenu;

    public VmMain(IPageRouter router)
    {
        this.Title = "Main View";
        this.ToggleMenuCommand = new RelayCommand(() => this.ShowMenu = !this.ShowMenu);

        router.Navigated += (sender, e) =>
        {
            if (this.showMenu)
            {
                this.ShowMenu = false;
            }
        };
    }

    public bool ShowMenu
    {
        get => this.showMenu;
        set => this.SetProperty(ref this.showMenu, value);
    }

    public ICommand ToggleMenuCommand { get; }

    //// --------------------------------------------------------------------------------------------
}
