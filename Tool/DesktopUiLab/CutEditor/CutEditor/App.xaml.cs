namespace CutEditor;

using System.IO;
using System.Windows;
using Cs.Logging;
using Cs.Logging.Providers;
using CutEditor.Services;
using CutEditor.ViewModel;
using Du.Core.Interfaces;
using Du.Excel;
using Du.Presentation.Util;
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
        services.AddTransient<FileLoader>();

        services.AddTransient<IUserInputProvider<string>, StringInputProvider>();
        services.AddSingleton<IContentDialogService, ContentDialogService>();
        services.AddTransient<IUserErrorNotifier, ErrorNotifierDialog>();
        services.AddTransient<ICollectionEditor, CollectionEditor>();
        services.AddTransient<IFilteredCollectionProvider, FilteredCollectionProvider>();
        services.AddTransient<IUserWaitingNotifier, WaitingNotifierDialog>();

        return services.BuildServiceProvider();
    }

    private void Initialize()
    {
        Log.Initialize(new SimpleFileLogProvider("log.txt"), LogLevelConfig.All);

        var loader = this.Services.GetRequiredService<FileLoader>();
        loader.Load();
    }
}
