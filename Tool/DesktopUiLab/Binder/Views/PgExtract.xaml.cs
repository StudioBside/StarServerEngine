namespace Binder.Views;

using System.Windows.Controls;
using Binder.ViewModels;
using Du.Core.Util;

public partial class PgExtract : Page
{
    public PgExtract()
    {
        this.InitializeComponent();
        this.DataContext = App.Current.Services.GetService<VmSingleBind>();
    }
}
