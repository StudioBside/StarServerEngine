namespace CutEditor.ViewModel;

using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Cs.Logging;
using Du.Core.Bases;
using Du.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Shared.Templet.Errors;

public sealed class VmErrors : VmPageBase
{
    private readonly IServiceProvider services;
    private readonly IFilteredCollection<ErrorMessage> filteredList;
    private ErrorType selectedFilter;

    public VmErrors(IServiceProvider services, IFilteredCollectionProvider provider)
    {
        this.services = services;
        this.Title = "오류 목록";
        this.filteredList = provider.Build(ErrorMessageController.Instance.List);
        this.filteredList.Filter = e =>
        {
            return this.selectedFilter == ErrorType.All || e.ErrorType == this.selectedFilter;
        };
    }

    public IEnumerable FilteredList => this.filteredList.List;
    public int FilteredCount => this.filteredList.FilteredCount;
    public int TotalCount => this.filteredList.SourceCount;

    public ErrorType SelectedFilter
    {
        get => this.selectedFilter;
        set => this.SetProperty(ref this.selectedFilter, value);
    }

    //// --------------------------------------------------------------------------------------------

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        switch (e.PropertyName)
        {
            case nameof(this.SelectedFilter):
                this.filteredList.Refresh();
                this.OnPropertyChanged(nameof(this.FilteredCount));
                break;
        }
    }
}
