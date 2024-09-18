namespace Binder;

using System.IO;
using System.Windows;
using Binder.Services;
using Binder.ViewModel;
using Cs.Logging;
using Cs.Logging.Providers;
using Du.Core.Interfaces;
using Du.Excel;
using Du.WpfLib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui;

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

        var workingDirectory = Directory.GetCurrentDirectory();
        var config = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(workingDirectory, "appsettings.json"), optional: false)
            //.AddJsonFile("appsettings.local.json", optional: true)
            .Build();

        services.AddTransient<IConfiguration>(_ => config);

        services.AddTransient<VmMain>();
        services.AddSingleton<VmHome>();
        services.AddTransient<VmSingleBind>();
        services.AddTransient<VmExtract>();
        services.AddTransient<FileLoader>();

        services.AddTransient<IUserInputProvider<string>, StringInputProvider>();
        services.AddSingleton<IContentDialogService, ContentDialogService>();
        services.AddSingleton<ISnackbarService, SnackbarService>();
        services.AddTransient<IUserErrorNotifier, ErrorNotifierDialog>();
        services.AddTransient<ICollectionEditor, CollectionEditor>();

        return services.BuildServiceProvider();
    }

    private void Initialize()
    {
        Log.Initialize(new SimpleFileLogProvider("log.txt"), LogLevelConfig.All);

        var loader = this.Services.GetRequiredService<FileLoader>();
        loader.Load();
    }
}
