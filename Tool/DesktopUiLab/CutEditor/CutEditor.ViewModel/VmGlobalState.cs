namespace CutEditor.ViewModel;

using System.ComponentModel;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Cs.Core;
using Cs.Core.Util;
using CutEditor.ViewModel.Detail;
using Microsoft.Extensions.Configuration;
using static CutEditor.Model.Messages;

public sealed class VmGlobalState : ObservableObject
{
    private bool showPreview;

    public static VmGlobalState Instance => Singleton<VmGlobalState>.Instance;
    public VmCuts.CrateParam? VmCutsCreateParam { get; private set; }
    public string TextFilePath { get; private set; } = string.Empty;

    public bool ShowPreview
    {
        get => this.showPreview;
        set => this.SetProperty(ref this.showPreview, value);
    }

    public void Initialize(IConfiguration config)
    {
        var templateSource = Assembly.GetExecutingAssembly().GetResourceString("CutEditor.ViewModel.TextTemplates.CutsOutput.stg");
        StringTemplateFactory.Instance.CreateFromString("CutsOutput", templateSource);

        this.TextFilePath = config["CutTextFilePath"] ?? throw new Exception("CutTextFilePath is not set in the configuration file.");
    }

    internal void ReserveVmCuts(VmCuts.CrateParam param)
    {
        // 마지막으로 reserve한 param은 계속 유지. history back -> forward 시에 사용
        this.VmCutsCreateParam = param;
    }

    //// --------------------------------------------------------------------------------------------

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        switch (e.PropertyName)
        {
            case nameof(this.ShowPreview):
                WeakReferenceMessenger.Default.Send(new PreviewChangedMessage(this.ShowPreview));
                break;
        }
    }
}
