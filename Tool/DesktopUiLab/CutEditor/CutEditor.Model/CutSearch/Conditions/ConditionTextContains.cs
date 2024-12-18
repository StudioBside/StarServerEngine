namespace CutEditor.Model.CutSearch.Conditions;

using System;
using CommunityToolkit.Mvvm.ComponentModel;

public sealed class ConditionTextContains : ObservableObject,
    ISearchCondition
{
    private string searchKeyword = string.Empty;

    bool ISearchCondition.IsValid => string.IsNullOrWhiteSpace(this.SearchKeyword) == false;

    public string SearchKeyword
    {
        get => this.searchKeyword;
        set => this.SetProperty(ref this.searchKeyword, value);
    }

    public bool IsTarget(Cut cut)
    {
        return cut.UnitTalk.IsTarget(this.SearchKeyword);
    }
}
