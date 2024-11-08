namespace CutEditor.Dialogs.BgFilePicker;

using System.Collections;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CutEditor.Model;
using CutEditor.Services;
using Du.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

public partial class BgFilePickerDialogVm : ObservableObject
{
    private readonly IFilteredCollection<ElementType> imageFiles;
    private readonly IFilteredCollection<ElementType> movFiles;
    private readonly IFilteredCollection<ElementType> slateFiles;
    private readonly IFilteredCollection<ElementType> spineFiles;
    private string searchKeyword = string.Empty;
    private string? selected;

    public BgFilePickerDialogVm()
    {
        var filteredCollectionProvider = App.Current.Services.GetRequiredService<IFilteredCollectionProvider>();

        ThumbnailMaker.UpdateAll();
        var imageList = AssetList.Instance.BgImageFiles.Select(e =>
        {
            var element = new ElementType(e) { Category = "Background" };
            element.LoadImage();
            return element;
        }).Concat(AssetList.Instance.StoryImageFiles.Select(e =>
        {
            var element = new ElementType(e) { Category = "Story" };
            element.LoadImage();
            return element;
        }));

        this.imageFiles = filteredCollectionProvider.Build(imageList);

        this.movFiles = filteredCollectionProvider.Build(AssetList.Instance.MovFiles.Select(e => new ElementType(e)));
        this.slateFiles = filteredCollectionProvider.Build(AssetList.Instance.SlateFiles.Select(e => new ElementType(e)));
        this.spineFiles = filteredCollectionProvider.Build(AssetList.Instance.SpineFiles.Select(e => new ElementType(e)));
    }

    public IEnumerable ImageFiles => this.imageFiles.List;
    public int ImageCount => this.imageFiles.FilteredCount;
    public IEnumerable MovFiles => this.movFiles.List;
    public int MovCount => this.movFiles.FilteredCount;
    public IEnumerable SlateFiles => this.slateFiles.List;
    public int SlateCount => this.slateFiles.FilteredCount;
    public IEnumerable SpineFiles => this.spineFiles.List;
    public int SpineCount => this.spineFiles.FilteredCount;
    public string? Selected
    {
        get => this.selected;
        set => this.SetProperty(ref this.selected, value);
    }

    public string SearchKeyword
    {
        get => this.searchKeyword;
        set => this.SetProperty(ref this.searchKeyword, value);
    }

    //// --------------------------------------------------------------------------------------------

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        switch (e.PropertyName)
        {
            case nameof(this.SearchKeyword):
                this.RefreshFilteredList();
                break;
        }
    }

    private void RefreshFilteredList()
    {
        this.imageFiles.Refresh(this.searchKeyword);
        this.movFiles.Refresh(this.searchKeyword);
        this.slateFiles.Refresh(this.searchKeyword);
        this.spineFiles.Refresh(this.searchKeyword);

        this.OnPropertyChanged(nameof(this.ImageCount));
        this.OnPropertyChanged(nameof(this.MovCount));
        this.OnPropertyChanged(nameof(this.SlateCount));
        this.OnPropertyChanged(nameof(this.SpineCount));
    }
}
