namespace Binder.Views;

using System.Windows.Controls;
using Binder.ViewModels;
using Du.Core.Util;

public partial class SingleBind : Page
{
    public SingleBind()
    {
        this.InitializeComponent();
        this.DataContext = App.Current.Services.GetService<VmSingleBind>();
    }
}
