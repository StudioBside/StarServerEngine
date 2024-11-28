namespace CutEditor.ViewModel;

using System;
using System.Collections;
using System.ComponentModel;
using Du.Core.Bases;
using Du.Core.Interfaces;
using Shared.Templet.Strings;

public sealed class VmStrings : VmPageBase
{
    private readonly IServiceProvider services;
    private readonly ISearchableCollection<StringElement> filteredList;
    private StringElement? selectedItem;
    private string searchKeyword = string.Empty;
    private bool showKorean = true;
    private bool showJapanese = false;
    private bool showEnglish = false;
    private bool showChinese = false;
    private bool showDupeOnly = false;

    public VmStrings(IServiceProvider services, ISearchableCollectionProvider provider)
    {
        this.services = services;
        this.Title = "시스템 스트링 리스트";
        this.filteredList = provider.Build(StringTable.Instance.Elements);
    }

    public IEnumerable FilteredList
    {
        get
        {
            var list = this.filteredList.TypedList;
            if (this.ShowDupeOnly)
            {
                return list.OrderByDescending(e => e.KeyCount);
            }
    
            return list;
        }
    }

    public int FilteredCount => this.filteredList.FilteredCount;
    public int TotalCount => StringTable.Instance.UniqueCount;
    public string SearchKeyword
    {
        get => this.searchKeyword;
        set => this.SetProperty(ref this.searchKeyword, value);
    }

    public bool ShowKorean
    {
        get => this.showKorean;
        set => this.SetProperty(ref this.showKorean, value);
    }

    public bool ShowJapanese
    {
        get => this.showJapanese;
        set => this.SetProperty(ref this.showJapanese, value);
    }

    public bool ShowEnglish
    {
        get => this.showEnglish;
        set => this.SetProperty(ref this.showEnglish, value);
    }

    public bool ShowChinese
    {
        get => this.showChinese;
        set => this.SetProperty(ref this.showChinese, value);
    }

    public bool ShowDupeOnly
    {
        get => this.showDupeOnly;
        set => this.SetProperty(ref this.showDupeOnly, value);
    }

    public StringElement? SelectedItem
    {
        get => this.selectedItem;
        set => this.SetProperty(ref this.selectedItem, value);
    }

    //// --------------------------------------------------------------------------------------------

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        switch (e.PropertyName)
        {
            case nameof(this.SearchKeyword):
            case nameof(this.ShowDupeOnly):
                this.filteredList.Refresh(this.searchKeyword);
                this.OnPropertyChanged(nameof(this.FilteredList));
                this.OnPropertyChanged(nameof(this.FilteredCount));
                break;
        }
    }
}
