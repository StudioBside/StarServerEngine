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

        this.CopyMessageCommand = new RelayCommand<ErrorMessage>(this.OnCopyMessage);
    }

    public IEnumerable FilteredList => this.filteredList.List;
    public int FilteredCount => this.filteredList.FilteredCount;
    public int TotalCount => this.filteredList.SourceCount;
    public ICommand CopyMessageCommand { get; }

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

    private void OnCopyMessage(ErrorMessage? message)
    {
        if (message is null)
        {
            return;
        }

        var writer = this.services.GetRequiredService<IClipboardWriter>();
        writer.SetText(message.Message);

        Log.Info($"메시지를 클립보드에 복사했습니다. \n {message.Message}");
    }
}
