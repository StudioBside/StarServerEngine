namespace Du.WpfLib;

using System.Windows.Controls;
using Wpf.Ui.Controls;

public partial class StringInputDialog : ContentDialog
{
    public StringInputDialog(ContentPresenter? contentPresenter, string title, string message, string defaultValue)
        : base(contentPresenter)
    {
        this.Title = title;
        this.Message = message;
        this.UserInput = defaultValue;

        this.DataContext = this;
        this.InitializeComponent();
    }

    public string Message { get; }
    public string UserInput { get; set; } = string.Empty;

    protected override void OnClosed(ContentDialogResult result)
    {
        if (result != ContentDialogResult.Primary)
        {
            this.UserInput = string.Empty;
        }

        base.OnClosed(result);
    }
}
