namespace CutEditor.Dialogs;

using System.ComponentModel;
using System.Windows.Controls;
using CutEditor.Model;
using Wpf.Ui.Controls;
using static CutEditor.Model.Enums;

public partial class BgFadeEditorDialog : ContentDialog
{
    public BgFadeEditorDialog(ContentPresenter? dialogHost, BgFadeInOut? current) : base(dialogHost)
    {
        this.DataContext = this;
        this.Data = current?.Clone() ?? new BgFadeInOut();

        this.InitializeComponent();

        this.StartColor.Initialize(this.Data.StartColor, this.Data.FadeType == BgFadeType.FadeIn);
        this.EndColor.Initialize(this.Data.EndColor, isEnabled: true);

        this.Data.PropertyChanged += this.Data_PropertyChanged;
    }

    public BgFadeInOut Data { get; }
    public float Alpha => this.Data.StartColor.A / 255f;

    //// --------------------------------------------------------------------------------------------

    protected override void OnButtonClick(ContentDialogButton button)
    {
        if (button == ContentDialogButton.Primary)
        {
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
