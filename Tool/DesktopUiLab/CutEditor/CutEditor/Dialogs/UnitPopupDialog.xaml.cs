namespace CutEditor.Dialogs;

using System.Windows.Controls;
using System.Windows.Input;
using Du.Presentation.Extensions;
using Shared.Templet.TempletTypes;
using Wpf.Ui.Controls;

public partial class UnitPopupDialog : ContentDialog
{
    public UnitPopupDialog(ContentPresenter? dialogHost, Unit unitTemplet) : base(dialogHost)
    {
        this.UnitTemplet = unitTemplet;

        this.DataContext = this;
        this.InitializeComponent();
    }

    public Unit UnitTemplet { get; }

    //// --------------------------------------------------------------------------------------------

    protected override void OnButtonClick(ContentDialogButton button)
    {
        base.OnButtonClick(button);

        if (button == ContentDialogButton.Secondary)
        {
            PageRouterExtension.Instance.Route(this.UnitTemplet);
        }
    }
}
