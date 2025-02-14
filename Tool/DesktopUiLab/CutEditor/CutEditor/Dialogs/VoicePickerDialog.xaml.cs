namespace CutEditor.Dialogs;

using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CutEditor.Model;
using CutEditor.Services;
using Du.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using NPOI.HPSF;
using Shared.Interfaces;
using Wpf.Ui.Controls;
using static StringStorage.Enums;

public partial class VoicePickerDialog : ContentDialog
{
    private readonly ISearchableCollection<VoiceSet> filteredList;
    private string searchKeyword = string.Empty;

    public VoicePickerDialog(
        ContentPresenter? contentPresenter)
        : base(contentPresenter)
    {
        this.DataContext = this;

        var voiceKoreanExists = VoiceL10nSorter.Instance.VoiceSetsKoreanExists;

        this.Title = $"보이스 선택 ({voiceKoreanExists.Count} groups)";
        this.filteredList = App.Current.Services.GetRequiredService<ISearchableCollectionProvider>().Build(voiceKoreanExists);

        this.InitializeComponent();
    }

    public IEnumerable FilteredFiles => this.filteredList.List;
    public VoiceSet? Selected { get; set; }
    public string SearchKeyword
    {
        get => this.searchKeyword;
        set => this.UpdateSearchKeyword(value);
    }

    //// --------------------------------------------------------------------------------------------

    protected override void OnButtonClick(ContentDialogButton button)
    {
        if (button == ContentDialogButton.Primary && this.Selected is null)
        {
            this.InfoBarWarning.Visibility = Visibility.Visible;
            return;
        }

        base.OnButtonClick(button);
    }

    private void UpdateSearchKeyword(string value)
    {
        this.searchKeyword = value;
        this.filteredList.Refresh(this.searchKeyword);
    }
}
