namespace Cs.Wpf.Bases;

using CommunityToolkit.Mvvm.ComponentModel;

public abstract partial class ViewModelBase : ObservableObject
{
    [ObservableProperty]
    private string title = string.Empty;
}
