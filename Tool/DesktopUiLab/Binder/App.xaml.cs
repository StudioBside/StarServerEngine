namespace Binder;

using System.IO;
using System.Windows;
using Binder.Services;
using Binder.ViewModel;
using Cs.Logging;
using Cs.Logging.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public partial class App : Application
{
    public App()
    {
        this.Services = ConfigureServices();
        this.Initialize();
        this.InitializeComponent();
    }

    public static new App Current => (App)Application.Current;
    public IServiceProvider Services { get; }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        var workingDirectory = System.IO.Directory.GetCurrentDirectory();
        var config = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(workingDirectory, "appsettings.json"), optional: false)
            //.AddJsonFile("appsettings.local.json", optional: true)
            .Build();

        services.AddTransient<IConfiguration>(_ => config);

        services.AddTransient(typeof(VmMain));
        services.AddSingleton(typeof(VmHome));
        services.AddTransient(typeof(VmSingleBind));
        services.AddTransient(typeof(VmExtract));
        services.AddTransient(typeof(FileLoader));

        return services.BuildServiceProvider();
    }

    private void Initialize()
    {
        Log.Initialize(new SimpleFileLogProvider("log.txt"), LogLevelConfig.All);

        var loader = this.Services.GetRequiredService<FileLoader>();
        loader.Load();
    }
}
