namespace Binder.Model;

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using Cs.Core.Util;
using Du.Core.Interfaces;

public sealed class BindFile : ObservableObject, ISearchable
{
    private readonly string filePath;
    private readonly ObservableCollection<Extract> extracts = new();
    private readonly List<ExtractEnum> extractEnums = new();
    private readonly List<ExtractString> extractStrings = new();
    private readonly List<ExtractHotswap> extractHotswaps = new();

    public BindFile(string filePath)
    {
        this.filePath = filePath;
        this.Name = Path.GetFileNameWithoutExtension(filePath);
        this.LastWriteTime = File.GetLastWriteTime(filePath);

        using var document = JsonHelper.LoadJsonc(filePath);
        var root = document.RootElement;
        root.TryGetArray("extracts", this.extracts, element => new Extract(element));
        root.TryGetArray("extractEnums", this.extractEnums, element => new ExtractEnum(element));
        root.TryGetArray("extractStrings", this.extractStrings, element => new ExtractString(element));
        root.TryGetArray("extractHotswap", this.extractHotswaps, element => new ExtractHotswap(element));
    }

    public string Name { get; }
    public DateTime LastWriteTime { get; }
    public IList<Extract> Extracts => this.extracts;
    public IList<ExtractEnum> ExtractEnums => this.extractEnums;
    public IList<ExtractString> ExtractStrings => this.extractStrings;
    public IList<ExtractHotswap> ExtractHotswaps => this.extractHotswaps;

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"파일 이름: {this.Name}");
        sb.AppendLine($"수정 시각: {this.LastWriteTime}");

        return sb.ToString();
    }

    public bool HasExtract(string outputFileName)
    {
        return this.extracts.Any(e => e.OutputFile.Equals(outputFileName, StringComparison.CurrentCultureIgnoreCase));
    }

    public bool IsTarget(string keyword)
    {
        if (this.Name.Contains(keyword, StringComparison.CurrentCultureIgnoreCase))
        {
            return true;
        }

        // 컬럼 검색 넣으려면.. 1. 모든 컬럼을 다 훑어야 하고, 2. ui에서 extract 리스트도 필터링 된 목록을 제공해야 함.
        ////foreach (var column in this.extracts.SelectMany(e => e.BindRoot.Columns))
        ////{
        ////    if (column.Name.Contains(keyword, StringComparison.CurrentCultureIgnoreCase))
        ////    {
        ////        return true;
        ////    }
        ////}

        return false;
    }
}