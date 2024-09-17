namespace Binder.ViewModel;

using System.Collections.ObjectModel;
using System.ComponentModel;
using Binder.Model;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Cs.Logging;
using Du.Core.Bases;
using Du.Core.Models;

public sealed partial class VmHome : VmPageBase
{
    private readonly List<BindFile> bindFiles = new();
    private ObservableCollection<BindFile> filteredFiles = new();
    private string searchKeyword = string.Empty;

    public VmHome()
    {
        this.Title = "바인딩 파일 목록";

        this.ExtractAddCommand = new RelayCommand(this.OnExtractAdd);
        this.ExtractEditCommand = new RelayCommand<Extract>(this.OnExtractEdit, _ => this.selectedExtract is not null);
        this.ExtractDeleteCommand = new RelayCommand(this.OnExtractDelete, () => this.selectedExtract is not null);

        this.ExtractEnumAddCommand = new RelayCommand(this.OnExtractEnumAdd);
        this.ExtractEnumEditCommand = new RelayCommand(this.OnExtractEnumEdit, () => this.selectedExtractEnum is not null);
        this.ExtractEnumDeleteCommand = new RelayCommand(this.OnExtractEnumDelete, () => this.selectedExtractEnum is not null);

        this.ExtractStringAddCommand = new RelayCommand(this.OnExtractStringAdd);
        this.ExtractStringEditCommand = new RelayCommand(this.OnExtractStringEdit, () => this.selectedExtractString is not null);
        this.ExtractStringDeleteCommand = new RelayCommand(this.OnExtractStringDelete, () => this.selectedExtractString is not null);

        this.ExtractHotswapAddCommand = new RelayCommand(this.OnExtractHotswapAdd);
        this.ExtractHotswapEditCommand = new RelayCommand(this.OnExtractHotswapEdit, () => this.selectedExtractHotswap is not null);
        this.ExtractHotswapDeleteCommand = new RelayCommand(this.OnExtractHotswapDelete, () => this.selectedExtractHotswap is not null);
    }

    public ObservableCollection<BindFile> FilteredFiles
    {
        get => this.filteredFiles;
        set => this.SetProperty(ref this.filteredFiles, value);
    }

    public string SearchKeyword
    {
        get => this.searchKeyword;
        set => this.SetProperty(ref this.searchKeyword, value);
    }

    public void AddFile(BindFile bindFile)
    {
        bindFile.PropertyChanged += this.OnBindFilePropertyChanged;
        this.bindFiles.Add(bindFile);

        this.FilterFiles();
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

            case nameof(this.SelectedExtractHotswap):
                this.ExtractHotswapEditCommand.NotifyCanExecuteChanged();
                this.ExtractHotswapDeleteCommand.NotifyCanExecuteChanged();
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
            this.FilteredFiles = new(this.bindFiles);
        }
        else
        {
            this.FilteredFiles = new(this.bindFiles.Where(x => x.IsTarget(this.searchKeyword)));
        }
    }

    private void OnExtractAdd()
    {
    }

    private void OnExtractEdit(Extract? extract)
    {
        if (this.selectedExtract is null)
        {
            Log.Error("Selected file is null");
            return;
        }

        WeakReferenceMessenger.Default.Send(new NavigationMessage("Views/PgExtract.xaml"));
    }

    private void OnExtractDelete()
    {
        if (this.selectedExtract is null || this.selectedBindFile is null)
        {
            Log.Error("Selected file or extract is null");
            return;
        }

        var bindFile = this.selectedBindFile;
        bindFile.Extracts.Remove(this.selectedExtract);
        this.SelectedExtract = null;
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

    private void OnExtractHotswapAdd()
    {
    }

    private void OnExtractHotswapEdit()
    {
    }

    private void OnExtractHotswapDelete()
    {
    }
}
