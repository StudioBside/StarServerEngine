namespace Binder.Views;

using System.Windows.Controls;
using Binder.ViewModels;

public partial class CustomerPage : Page
{
    public CustomerPage()
    {
        this.InitializeComponent();
        this.DataContext = App.Current.Services.GetService(typeof(VmCustomer));
    }
}
