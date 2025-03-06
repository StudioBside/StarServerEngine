namespace CutEditor;

using System.IO;
using System.Windows;
using Cs.Logging;
using CutEditor.Behaviors;
using CutEditor.Model;
using CutEditor.Model.CutSearch;
using CutEditor.Model.Detail;
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
using Shared.Templet.UnitScripts;
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

        services.AddTransient<IConfiguration>(_ => config);

        services.AddTransient<VmMain>();
        services.AddSingleton<AssetList>();
        services.AddSingleton<IPageRouter>(PageRouterExtension.Instance);
        services.AddTransient<VmHome>();
        services.AddTransient<VmCuts.Factory>();
        services.AddTransient<VmCutsSummary.Factory>();
        services.AddTransient<VmL10n>();
        services.AddTransient<VmStrings>();
        services.AddTransient<VmUnits>();
        services.AddTransient<VmSkills>();
        services.AddTransient<VmErrors>();
        services.AddTransient<VmUnitScripts>();
        services.AddTransient<VmBuffs>();
        services.AddSingleton<VmCutSearch>();
        services.AddTransient<FileAndSnackbarLog>();
        services.AddTransient<IGeneralPicker<Unit>, UnitPicker>();
        services.AddTransient<IGeneralPicker<UnitVariant>, UnitVariantPicker>();
        services.AddTransient<IGeneralPicker<LobbyItem>, ArcpointPicker>();
        services.AddTransient<IExcelFileWriter, ExcelFileWriter>();
        services.AddTransient<IExcelFileReader, ExcelFileReader>();
        services.AddSingleton<IEnumPicker<Ease>>(EasingGraph.Instance);
        services.AddSingleton<IEnumPicker<CameraOffset>>(CameraOffsetController.Instance);
        services.AddSingleton(CutsListController.LastInstance);
        services.AddKeyedTransient<IAssetPicker, BgmPicker>("bgm");
        services.AddKeyedTransient<IAssetPicker, SfxPicker>("sfx");
        services.AddKeyedTransient<IAssetPicker, VoicePicker>("voice");
        services.AddKeyedTransient<IAssetPicker, BgFilePicker>("bgFile");
        services.AddKeyedTransient<IModelEditor<IList<StringElement>>, UnitNameEditor>("unitName");
        services.AddTransient<IModelEditor<BgFadeInOut>, BgFadeEditor>();
        services.AddTransient<IUnitReplaceQuery, UnitReplaceQuery>();
        services.AddTransient<IUnitPopup, UnitPopup>();
        services.AddScoped<UndoController>();
        services.AddSingleton<IClipboardWrapper, ClipboardWrapper>();

        services.AddTransient<IUserInputProvider<string>, StringInputProvider>();
        services.AddTransient<IUserInputProvider<bool>, BooleanInputProvider>();
        services.AddTransient<IDialogProvider, DialogProvider>();
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

    private void Initialize()
    {
        Log.Initialize(this.Services.GetService<FileAndSnackbarLog>(), LogLevelConfig.All);

        var assetList = this.Services.GetRequiredService<AssetList>();
        EasingGraph.Instance.Initialize(assetList);
        CameraOffsetController.Instance.Initialize(assetList);
        VoiceL10nSorter.Instance.Initialize(assetList);

        var config = this.Services.GetRequiredService<IConfiguration>();
        VmGlobalState.Instance.Initialize(config);
        TempletLoad.Execute(config);

        CutSceneContainer.Instance.Load(VmGlobalState.Instance.CutSceneDataFilePath);

        if (ErrorContainer.HasError)
        {
            Log.ErrorAndExit($"{ErrorContainer.ErrorCount}개의 에러 발생");
        }

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
            .Register<PgCuts, VmCutSummary>()
            .Register<PgCuts, CutSearchResult>()
            .Register<PgCutsSummary, VmCutsSummary.CreateParam>()
            .Register<PgUnitDetail, Unit>()
            .Register<PgUnitScriptDetail, UnitScript>()
            .Register<PgBuffDetail, BuffTemplet>()
            ;
    }
}
