namespace CutEditor.Dialogs;

using System.ComponentModel;
using System.Windows.Controls;
using Cs.Logging;
using Cs.Math;
using CutEditor.Model;
using Wpf.Ui.Controls;
using static CutEditor.Model.Enums;

public partial class BgFadeEditorDialog : ContentDialog
{
    public BgFadeEditorDialog(ContentPresenter? dialogHost, BgFadeInOut? current) : base(dialogHost)
    {
        this.Data = current?.Clone() ?? new BgFadeInOut();
        this.Data.PropertyChanged += this.Data_PropertyChanged;

        this.DataContext = this.Data;
        this.InitializeComponent();

        this.StartColor.Initialize(this.Data.StartColor, this.Data.FadeType == BgFadeType.FadeIn);
        this.EndColor.Initialize(this.Data.EndColor, isEnabled: true);
    }

    public BgFadeInOut Data { get; }

    //// --------------------------------------------------------------------------------------------

    protected override void OnButtonClick(ContentDialogButton button)
    {
        if (button == ContentDialogButton.Primary)
        {
            if (this.Data.Time.IsNearlyZero())
            {
                Log.Warn("시간이 유효하지 않습니다.");
                return;
            }

            this.Data.StartColor = this.StartColor.GetDrawingColor();
            this.Data.EndColor = this.EndColor.GetDrawingColor();
        }

        base.OnButtonClick(button);
    }

    private void Data_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(BgFadeInOut.FadeType):
                this.StartColor.IsEnabled = this.Data.FadeType == BgFadeType.FadeIn;
                break;
        }
    }
}
