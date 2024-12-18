namespace CutEditor.ViewModel;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Cs.Logging;
using CutEditor.Model;
using CutEditor.Model.CutSearch;
using CutEditor.Model.CutSearch.Conditions;
using CutEditor.ViewModel.Detail;
using Du.Core.Bases;
using static CutEditor.Model.Enums;

public sealed class VmCutSearch : VmPageBase
{
    private readonly IServiceProvider services;
    private readonly ObservableCollection<CutSearchResult> searchResults = new();
    private readonly ObservableCollection<ISearchCondition> searchConditions = new();
    private SearchCombinationType searchCombinationType;
    private double searchElapsedSec;
    private DateTime searchTime;

    public VmCutSearch(IServiceProvider services)
    {
        this.Title = "컷신 데이터 검색";
        this.services = services;

        this.AddTextConditionCommand = new RelayCommand(() => this.AddCondition(new ConditionTextContains()));
        this.AddUnitConditionCommand = new RelayCommand(() => this.AddCondition(new ConditionUnitMatch(services)));
        this.RemoveConditionCommand = new RelayCommand<ISearchCondition>(this.OnRemoveCondition);
        this.StartSearchCommand = new RelayCommand(this.OnStartSearch);
        this.SetCombinationTypeCommand = new RelayCommand<string>(this.OnSetCombinationType);

        this.AddCondition(new ConditionTextContains());
    }

    public IList<CutSearchResult> SearchResults => this.searchResults;
    public IList<ISearchCondition> SearchConditions => this.searchConditions;

    public SearchCombinationType SearchCombinationType
    {
        get => this.searchCombinationType;
        set => this.SetProperty(ref this.searchCombinationType, value);
    }

    public DateTime SearchTime
    {
        get => this.searchTime;
        set => this.SetProperty(ref this.searchTime, value);
    }

    public double SearchElapsedSec
    {
        get => this.searchElapsedSec;
        set => this.SetProperty(ref this.searchElapsedSec, value);
    }

    public ICommand AddTextConditionCommand { get; }
    public ICommand AddUnitConditionCommand { get; }
    public ICommand RemoveConditionCommand { get; }
    public ICommand StartSearchCommand { get; }
    public ICommand SetCombinationTypeCommand { get; }

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
        var validConditions = this.SearchConditions.Where(e => e.IsValid).ToArray();
        if (validConditions.Length == 0)
        {
            Log.Warn("검색 조건이 유효하지 않습니다.");
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        this.searchResults.Clear();
        var keywords = validConditions.OfType<ConditionTextContains>()
            .Select(e => e.SearchKeyword)
            .ToArray();

        foreach (var cutscene in CutSceneContainer.Instance.CutScenes)
        {
            var cuts = CutFileIo.LoadCutData(cutscene.FileName);
            var uidGenerator = new CutUidGenerator(cuts);
            foreach (var cut in cuts.Where(e => e.Choices.Any()))
            {
                var choiceUidGenerator = new ChoiceUidGenerator(cut.Uid, cut.Choices);
            }

            foreach (var cut in cuts.Where(e => IsTarget(e, this.searchCombinationType, validConditions)))
            {
                this.searchResults.Add(new CutSearchResult(cutscene, cut, keywords));
            }
        }

        this.SearchElapsedSec = stopwatch.Elapsed.TotalSeconds;
        this.SearchTime = DateTime.Now;

        static bool IsTarget(Cut cut, SearchCombinationType combinationType, IEnumerable<ISearchCondition> conditions)
        {
            if (combinationType == SearchCombinationType.And)
            {
                return conditions.All(e => e.IsTarget(cut));
            }

            return conditions.Any(e => e.IsTarget(cut));
        }
    }

    private void OnRemoveCondition(ISearchCondition? condition)
    {
        if (condition is null)
        {
            return;
        }

        this.searchConditions.Remove(condition);
    }

    private void AddCondition(ISearchCondition newCondition)
    {
        const int MaxCount = 5;
        if (this.searchConditions.Count >= MaxCount)
        {
            Log.Warn($"검색 조건의 조합은 최대 {MaxCount}개로 제한합니다.");
            return;
        }

        this.searchConditions.Add(newCondition);
    }

    private void OnSetCombinationType(string? typeStr)
    {
        if (Enum.TryParse<SearchCombinationType>(typeStr, out var type) == false)
        {
            Log.Warn($"검색 조합 타입을 파싱할 수 없습니다: {typeStr}");
            return;
        }

        this.SearchCombinationType = type;
    }
}