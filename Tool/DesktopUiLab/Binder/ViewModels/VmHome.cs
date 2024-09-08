namespace Binder.ViewModels;

using System.Collections.ObjectModel;
using Binder.Models;
using Du.Core.Bases;

public sealed class VmHome : VmPagelBase
{
    private readonly ObservableCollection<BindFile> bindFiles = new();

    public VmHome()
    {
        this.Title = "바인딩 파일 목록";
        this.bindFiles.Add(new BindFile("hello world"));
    }

    public IList<BindFile> BindFiles => this.bindFiles;
}
