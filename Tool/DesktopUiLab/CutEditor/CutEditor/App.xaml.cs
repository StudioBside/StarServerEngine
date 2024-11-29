namespace CutEditor;

using System.IO;
using System.Windows;
using Cs.Logging;
using CutEditor.Behaviors;
using CutEditor.Model;
using CutEditor.Model.Interfaces;
using CutEditor.Services;
using CutEditor.ViewModel;
using CutEditor.Views;
using Du.Core.Interfaces;
using Du.Core.Util;
using Du.Excel;
using Du.Presentation.Extensions;
using Du.Presentation.Util;
using Du.WpfLib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NKM;
using Shared.Templet;
using Shared.Templet.Strings;
using Shared.Templet.TempletTypes;
using Wpf.Ui;
using static CutEditor.Model.Enums;

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
        services.AddSingleton<IPageRouter>(PageRouterExtension.Instance);
        services.AddTransient<VmCuts.Factory>();
        services.AddTransient<VmCutsSummary.Factory>();
        services.AddTransient<VmStrings>();
        services.AddTransient<VmUnits>();
        services.AddTransient<VmErrors>();
        services.AddTransient<FileLoader>();
        services.AddTransient<FileAndSnackbarLog>();
        services.AddTransient<ITempletPicker<Unit>, UnitPicker>();
        services.AddTransient<ITempletPicker<LobbyItem>, ArcpointPicker>();
        services.AddTransient<IExcelFileWriter, ExcelFileWriter>();
        services.AddSingleton<IEnumPicker<Ease>>(EasingGraph.Instance);
        services.AddSingleton<IEnumPicker<CameraOffset>>(CameraOffsetController.Instance);
        services.AddSingleton(CutsListController.LastInstance);
        services.AddKeyedTransient<IAssetPicker, BgmPicker>("bgm");
        services.AddKeyedTransient<IAssetPicker, SfxPicker>("sfx");
        services.AddKeyedTransient<IAssetPicker, VoicePicker>("voice");
        services.AddKeyedTransient<IAssetPicker, BgFilePicker>("bgFile");
        services.AddKeyedTransient<IModelEditor<IList<StringElement>>, UnitNameEditor>("unitName");
        services.AddTransient<IModelEditor<BgFadeInOut>, BgFadeEditor>();
        services.AddScoped<UndoController>();

        services.AddTransient<IUserInputProvider<string>, StringInputProvider>();
        services.AddTransient<IUserInputProvider<bool>, BooleanInputProvider>();
        services.AddSingleton<IContentDialogService, ContentDialogService>();
        services.AddSingleton<ISnackbarService, SnackbarService>();
        services.AddTransient<IPopupMessageNotifier, PopupMessageNotifier>();
        services.AddTransient<ICollectionEditor, CollectionEditor>();
        services.AddTransient<ISearchableCollectionProvider, SearchableCollectionProvider>();
        services.AddTransient<IFilteredCollectionProvider, FilteredCollectionProvider>();
        services.AddTransient<IUserWaitingNotifier, WaitingNotifierDialog>();
        services.AddTransient<IFilePicker, FilePicker>();

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

        var assetList = this.Services.GetRequiredService<AssetList>();
        EasingGraph.Instance.Initialize(assetList);
        CameraOffsetController.Instance.Initialize(assetList);

        var config = this.Services.GetRequiredService<IConfiguration>();
        VmGlobalState.Instance.Initialize(config);
        TempletLoad.Execute(config);

        var loader = this.Services.GetRequiredService<FileLoader>();
        loader.Load();

        if (Directory.Exists(ImageHelper.ThumbnailRoot) == false)
        {
            Log.Debug($"Creating thumbnail directory: {ImageHelper.ThumbnailRoot}");
            ThumbnailMaker.UpdateAll();
        }

        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            File.WriteAllText("crash_log.txt", e.ExceptionObject.ToString());
        };

        PageRouterExtension.Instance
            .Register<PgCuts, VmCuts.CreateParam>()
            .Register<PgCuts, CutScene>()
            .Register<PgCuts, VmCut>()
            .Register<PgCutsSummary, VmCutsSummary.CreateParam>()
            ;
    }
}
