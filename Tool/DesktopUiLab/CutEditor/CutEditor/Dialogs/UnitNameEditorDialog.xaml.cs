namespace CutEditor.Dialogs;

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Cs.Logging;
using Du.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Shared.Templet.Strings;
using Wpf.Ui.Controls;

public partial class UnitNameEditorDialog : ContentDialog
{
    public const int MaxStringLength = 15;
    private readonly ObservableCollection<StringElement> selectedList = new();
    private readonly IFilteredCollection<StringElement> filteredList;
    private string searchKeyword = string.Empty;

    public UnitNameEditorDialog(ContentPresenter? dialogHost, IList<StringElement> current) : base(dialogHost)
    {
        foreach (var element in current)
        {
            this.selectedList.Add(element);
        }

        // candidate 목록 생성
        var list = StringTable.Instance.Elements.Where(e => e.Korean.Length <= MaxStringLength);
        this.filteredList = App.Current.Services.GetRequiredService<IFilteredCollectionProvider>().Build(list);
        this.AddCommand = new RelayCommand<StringElement>(this.OnAdd);
        this.RemoveCommand = new RelayCommand<StringElement>(this.OnRemove);

        this.DataContext = this;
        this.InitializeComponent();
    }

    public IList<StringElement> SelectedList => this.selectedList;
    public IEnumerable FilteredFiles => this.filteredList.List;
    public ICommand AddCommand { get; }
    public ICommand RemoveCommand { get; }

    public string SearchKeyword
    {
        get => this.searchKeyword;
        set => this.UpdateSearchKeyword(value);
    }

    //// --------------------------------------------------------------------------------------------

    protected override void OnButtonClick(ContentDialogButton button)
    {
        if (button == ContentDialogButton.Primary && this.selectedList.Count == 0)
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

    private void OnAdd(StringElement? element)
    {
        if (element is null || this.selectedList.Contains(element))
        {
            return;
        }

        this.selectedList.Add(element);
    }

    private void OnRemove(StringElement? element)
    {
        if (element is null)
        {
            return;
        }

        this.selectedList.Remove(element);
    }
}
