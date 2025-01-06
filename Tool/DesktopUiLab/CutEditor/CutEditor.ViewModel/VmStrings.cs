namespace CutEditor.ViewModel;

using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Cs.Logging;
using CutEditor.Model;
using CutEditor.Model.Detail;
using Du.Core.Bases;
using Du.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Shared.Templet.Strings;

public sealed class VmStrings : VmPageBase
{
    private const string DefaultFilter = "[All]";
    private readonly IServiceProvider services;
    private readonly ISearchableCollection<StringElement> filteredList;
    private StringElement? selectedItem;
    private string searchKeyword = string.Empty;
    private bool showKorean = true;
    private bool showJapanese = false;
    private bool showEnglish = false;
    private bool showChinese = false;
    private bool showDupeOnly = false;
    private string selectedFilter = DefaultFilter;

    public VmStrings(IServiceProvider services, ISearchableCollectionProvider provider)
    {
        this.services = services;
        this.Title = "시스템 스트링 리스트";
        this.filteredList = provider.Build(StringTable.Instance.Elements);
        this.Filters = new string[] { DefaultFilter }.Concat(StringTable.Instance.CategoryNames).ToArray();
        this.ExportCommand = new RelayCommand(this.OnExport, () => this.selectedFilter != DefaultFilter);

        this.filteredList.SetSubFilter(e =>
        {
            return this.selectedFilter == DefaultFilter || e.CategoryName == this.selectedFilter;
        });
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
    public int TotalCount => this.filteredList.SourceCount;
    public IRelayCommand ExportCommand { get; }
    public IReadOnlyList<string> Filters { get; }
    public string ExportRoot => VmGlobalState.ExportRoot;

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

    public string SelectedFilter
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
            case nameof(this.SearchKeyword):
            case nameof(this.ShowDupeOnly):
            case nameof(this.SelectedFilter):
                this.filteredList.Refresh(this.searchKeyword);
                this.OnPropertyChanged(nameof(this.FilteredList));
                this.OnPropertyChanged(nameof(this.FilteredCount));

                this.ExportCommand.NotifyCanExecuteChanged();
                break;
        }
    }

    private void OnExport()
    {
        if (this.selectedFilter == DefaultFilter)
        {
            Log.Error($"필터에서 추출할 카테고리를 선택해야 합니다.");
            return;
        }

        if (StringTable.Instance.TryGetCategory(this.selectedFilter, out var category) == false)
        {
            Log.Error($"시스템 스트링 카테고리를 찾을 수 없습니다. category:{this.selectedFilter}");
            return;
        }

        Log.Debug($"시스템 스트링 추출. category:{category.Name}");
        var fileName = $"{this.ExportRoot}/SYSTEM_STRING_{category.Name}.xlsx";
        var nameOnly = Path.GetFileNameWithoutExtension(fileName);

        // 동일한 이름의 파일이 존재한다면 삭제.
        if (File.Exists(fileName))
        {
            File.Delete(fileName);
        }
 
        var writer = this.services.GetRequiredService<IExcelFileWriter>();
        if (writer.Write(fileName, category.Elements.Select(e => new StringOutputExcelFormat(e))) == false)
        {
            Log.Error($"파일 생성에 실패했습니다. fileName:{nameOnly}");
            return;
        }

        Log.Info($"파일 생성 완료. fileName:{nameOnly}");
    }
}
