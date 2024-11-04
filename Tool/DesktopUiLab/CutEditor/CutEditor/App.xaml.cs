namespace CutEditor;

using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using Cs.Logging;
using Cs.Logging.Providers;
using CutEditor.Dialogs;
using CutEditor.Dialogs.BgFilePicker;
using CutEditor.Model;
using CutEditor.Model.Interfaces;
using CutEditor.Services;
using CutEditor.ViewModel;
using Du.Core.Interfaces;
using Du.Core.Util;
using Du.Excel;
using Du.Presentation.Util;
using Du.WpfLib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Templet;
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

        VerifyConfiguration(config);

        services.AddTransient<IConfiguration>(_ => config);

        services.AddTransient<VmMain>();
        services.AddSingleton<VmHome>();
        services.AddSingleton<AssetList>();
        services.AddTransient<VmCuts>();
        services.AddTransient<FileLoader>();
        services.AddTransient<FileAndSnackbarLog>();
        services.AddTransient<IUnitPicker, UnitPicker>();
        services.AddTransient<IArcpointPicker, ArcpointPicker>();
        services.AddKeyedTransient<IAssetPicker, BgmPicker>("bgm");
        services.AddKeyedTransient<IAssetPicker, SfxPicker>("sfx");
        services.AddKeyedTransient<IAssetPicker, VoicePicker>("voice");
        services.AddKeyedTransient<IAssetPicker, BgFilePicker>("bgFile");
        services.AddScoped<UndoController>();

        services.AddTransient<IUserInputProvider<string>, StringInputProvider>();
        services.AddTransient<IUserInputProvider<bool>, BooleanInputProvider>();
        services.AddSingleton<IContentDialogService, ContentDialogService>();
        services.AddSingleton<ISnackbarService, SnackbarService>();
        services.AddTransient<IUserErrorNotifier, ErrorNotifierDialog>();
        services.AddTransient<ICollectionEditor, CollectionEditor>();
        services.AddTransient<IFilteredCollectionProvider, FilteredCollectionProvider>();
        services.AddTransient<IUserWaitingNotifier, WaitingNotifierDialog>();

        return services.BuildServiceProvider();
    }

    private static void VerifyConfiguration(IConfiguration config)
    {
        if (config["CutTextFilePath"] is null)
        {
            Log.ErrorAndExit("CutTextFilePath is not set in appsettings.json");
        }

        if (config["CutBinFilePath"] is null)
        {
            Log.ErrorAndExit("CutBinFilePath is not set in appsettings.json");
        }

        var textFilePacker = config["TextFilePacker"] ?? throw new Exception("TextFilePacker is not set in appsettings.json");
        if (File.Exists(textFilePacker) == false)
        {
            Log.ErrorAndExit($"TextFilePacker does not exist. config:{textFilePacker}");
        }
    }

    private void Initialize()
    {
        Log.Initialize(this.Services.GetService<FileAndSnackbarLog>(), LogLevelConfig.All);

        VmGlobalState.Instance.Initialize();
        TempletLoad.Execute(this.Services.GetRequiredService<IConfiguration>());

        var loader = this.Services.GetRequiredService<FileLoader>();
        loader.Load();

        var assetList = this.Services.GetRequiredService<AssetList>();
        if (Directory.Exists(ImageHelper.ThumbnailRoot) == false)
        {
            Log.Debug($"Creating thumbnail directory: {ImageHelper.ThumbnailRoot}");
            ThumbnailMaker.UpdateAll();
        }
    }
}
