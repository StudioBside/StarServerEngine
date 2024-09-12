namespace Binder.ViewModels;

using System.ComponentModel;
using System.Windows.Data;
using Binder.Models;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Cs.Logging;
using Du.Core.Bases;
using Du.Core.Models;
using Du.Core.Util;

public sealed partial class VmHome : VmPageBase
{
    private readonly List<BindFile> bindFiles = new();
    private string searchKeyword = string.Empty;
    private ListCollectionView filteredFilesView;

    public VmHome()
    {
        this.Title = "바인딩 파일 목록";
        this.filteredFilesView = new ListCollectionView(this.bindFiles);

        this.ExtractAddCommand = new RelayCommand(this.OnExtractAdd);
        this.ExtractEditCommand = new RelayCommand(this.OnExtractEdit, () => this.selectedExtract is not null);
        this.ExtractDeleteCommand = new RelayCommand(this.OnExtractDelete, () => this.selectedExtract is not null);

        this.ExtractEnumAddCommand = new RelayCommand(this.OnExtractEnumAdd);
        this.ExtractEnumEditCommand = new RelayCommand(this.OnExtractEnumEdit, () => this.selectedExtractEnum is not null);
        this.ExtractEnumDeleteCommand = new RelayCommand(this.OnExtractEnumDelete, () => this.selectedExtractEnum is not null);

        this.ExtractStringAddCommand = new RelayCommand(this.OnExtractStringAdd);
        this.ExtractStringEditCommand = new RelayCommand(this.OnExtractStringEdit, () => this.selectedExtractString is not null);
        this.ExtractStringDeleteCommand = new RelayCommand(this.OnExtractStringDelete, () => this.selectedExtractString is not null);
    }

    public ICollectionView FilteredFiles => this.filteredFilesView;
    public string SearchKeyword
    {
        get => this.searchKeyword;
        set => this.SetProperty(ref this.searchKeyword, value);
    }

    public void AddFile(BindFile bindFile)
    {
        bindFile.PropertyChanged += this.OnBindFilePropertyChanged;
        this.bindFiles.Add(bindFile);
    }

    //// --------------------------------------------------------------------------------------------

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        switch (e.PropertyName)
        {
            case nameof(this.SearchKeyword):
                this.FilterFiles();
                break;

            case nameof(this.SelectedBindFile):
                this.SelectedExtract = null;
                break;

            case nameof(this.SelectedExtract):
                this.ExtractEditCommand.NotifyCanExecuteChanged();
                this.ExtractDeleteCommand.NotifyCanExecuteChanged();
                break;

            case nameof(this.SelectedExtractEnum):
                this.ExtractEnumEditCommand.NotifyCanExecuteChanged();
                this.ExtractEnumDeleteCommand.NotifyCanExecuteChanged();
                break;

            case nameof(this.SelectedExtractString):
                this.ExtractStringEditCommand.NotifyCanExecuteChanged();
                this.ExtractStringDeleteCommand.NotifyCanExecuteChanged();
                break;
        }
    }

    private void OnBindFilePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
    }

    private void FilterFiles()
    {
        if (string.IsNullOrEmpty(this.searchKeyword))
        {
            this.filteredFilesView.Filter = null;
            return;
        }

        this.FilteredFiles.Filter = (obj) =>
        {
            if (obj is not BindFile bindFile)
            {
                return false;
            }

            return bindFile.Name.Contains(this.searchKeyword, StringComparison.CurrentCultureIgnoreCase);
        };
    }

    private void OnExtractAdd()
    {
    }

    private void OnExtractEdit()
    {
        if (this.selectedBindFile is null)
        {
            Log.Error("Selected file is null");
            return;
        }

        App.Current.Services.GetService<VmSingleBind>().BindFile = this.selectedBindFile;
        WeakReferenceMessenger.Default.Send(new NavigationMessage("Views/PgExtract.xaml"));
    }

    private void OnExtractDelete()
    {
    }

    private void OnExtractEnumAdd()
    {
    }

    private void OnExtractEnumEdit()
    {
    }

    private void OnExtractEnumDelete()
    {
    }

    private void OnExtractStringAdd()
    {
    }

    private void OnExtractStringEdit()
    {
    }

    private void OnExtractStringDelete()
    {
    }
}
