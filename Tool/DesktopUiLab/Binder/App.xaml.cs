namespace Binder;

using System.Configuration;
using System.Windows;
using Binder.ViewModels;
using Microsoft.Extensions.DependencyInjection;

public partial class App : Application
{
    public App()
    {
        this.Services = ConfigureServices();
        this.InitializeComponent();
    }

    public static new App Current => (App)Application.Current;
    public IServiceProvider Services { get; }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddTransient(typeof(VmMain));
        services.AddTransient(typeof(VmHome));
        services.AddTransient(typeof(VmCustomer));

        return services.BuildServiceProvider();
    }
}
