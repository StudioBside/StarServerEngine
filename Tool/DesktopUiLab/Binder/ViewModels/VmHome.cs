namespace Binder.ViewModels;

using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using Binder.Models;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Cs.Logging;
using Du.Core.Bases;
using Du.Core.Models;
using Du.Core.Util;

public sealed class VmHome : VmPagelBase
{
    private readonly List<BindFile> bindFiles = new();
    private string searchKeyword = string.Empty;
    private ListCollectionView filteredFilesView;
    private BindFile? selected;

    public VmHome()
    {
        this.Title = "바인딩 파일 목록";
        this.filteredFilesView = new ListCollectionView(this.bindFiles);
        this.EditCommand = new RelayCommand(this.OnEdit);
    }

    public ICollectionView FilteredFiles => this.filteredFilesView;
    public string SearchKeyword
    {
        get => this.searchKeyword;
        set
        {
            this.SetProperty(ref this.searchKeyword, value);
            this.FilterFiles();
        }
    }

    public BindFile? Selected
    {
        get => this.selected;
        set => this.SetProperty(ref this.selected, value);
    }

    public ICommand EditCommand { get; set; }

    public void AddFile(BindFile bindFile)
    {
        this.bindFiles.Add(bindFile);
    }

    //// --------------------------------------------------------------------------------------------
 
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

    private void OnEdit()
    {
        if (this.selected is null)
        {
            Log.Error("Selected file is null");
            return;
        }

        App.Current.Services.GetService<VmSingleBind>().BindFile = this.selected;
        WeakReferenceMessenger.Default.Send(new NavigationMessage("Views/SingleBind.xaml"));
    }
}
