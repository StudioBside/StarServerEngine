namespace CutEditor.ViewModel;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Cs.Logging;
using CutEditor.Model.CutSearch;
using Du.Core.Bases;
using static CutEditor.Model.Enums;

public sealed class VmCutSearch : VmPageBase
{
    private readonly IServiceProvider services;
    private readonly ObservableCollection<CutSearchResultGroup> searchResultGroups = new();
    private readonly ObservableCollection<ISearchCondition> searchConditions = new();
    private SearchCombinationType searchCombinationType;

    public VmCutSearch(IServiceProvider services)
    {
        this.Title = "컷신 데이터 검색";
        this.services = services;

        this.StartSearchCommand = new RelayCommand(this.OnStartSearch);
    }

    public IList<CutSearchResultGroup> SearchResultGroups => this.searchResultGroups;
    public IList<ISearchCondition> SearchConditions => this.searchConditions;

    public SearchCombinationType SearchCombinationType
    {
        get => this.searchCombinationType;
        set => this.SetProperty(ref this.searchCombinationType, value);
    }

    public ICommand StartSearchCommand { get; }

    //// --------------------------------------------------------------------------------------------

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        //switch (e.PropertyName)
        //{
        //    case nameof(this.SearchKeyword):
        //        break;
        //}
    }

    private void OnStartSearch()
    {
        if (this.SearchConditions.Count == 0)
        {
            Log.Error("검색 조건이 없습니다.");
            return;
        }
    }
}