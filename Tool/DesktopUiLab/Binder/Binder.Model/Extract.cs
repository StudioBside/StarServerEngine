namespace Binder.Model;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using Binder.Model.Detail;
using CommunityToolkit.Mvvm.ComponentModel;
using Du.Core.Util;
using static Binder.Model.Enums;

public sealed class Extract : ObservableObject
{
    private readonly ObservableCollection<Source> sources = new();
    private readonly ObservableCollection<Uniqueness> uniquenesses = new();
    private string outputFile = string.Empty;
    private string outputGroupBy = string.Empty;
    private string outputFilePrefix = string.Empty;
    private OutputDirection fileOutDirection = OutputDirection.All;
    private bool excludeToolOutput;
    ////// clientOutputType
    private BindRoot bindRoot;
    private CustomOutputPath? customOutputPath;
    private DuplicationCleaner? duplicationCleaner;

    public Extract(JsonElement element)
    {
        this.outputFile = element.GetString("outputFile");
        this.outputGroupBy = element.GetString("outputGroupBy", string.Empty);
        element.GetArray("sources", this.sources, element => new Source(element));
        element.GetArray("uniquenesses", this.uniquenesses, element => new Uniqueness(element));
        this.outputFilePrefix = element.GetString("outputFilePrefix", string.Empty);
        this.fileOutDirection = element.GetEnum<OutputDirection>("fileOutDirection");
        this.excludeToolOutput = element.GetBoolean("excludeToolOutput", false);
        this.bindRoot = new BindRoot(element.GetProperty("bindRoot"));
        if (element.TryGetProperty("customOutputPath", out var subElement))
        {
            this.customOutputPath = new CustomOutputPath(subElement);
        }

        if (element.TryGetProperty("duplicationCleaner", out subElement))
        {
            this.duplicationCleaner = new DuplicationCleaner(subElement);
        }
    }

    public IList<Source> Sources => this.sources;
    public IList<Uniqueness> Uniquenesses => this.uniquenesses;
    public BindRoot BindRoot => this.bindRoot;
    public string OutputFile
    {
        get => this.outputFile;
        set => this.SetProperty(ref this.outputFile, value);
    }

    public string OutputGroupBy
    {
        get => this.outputGroupBy;
        set => this.SetProperty(ref this.outputGroupBy, value);
    }

    public string OutputFilePrefix
    {
        get => this.outputFilePrefix;
        set => this.SetProperty(ref this.outputFilePrefix, value);
    }

    public OutputDirection FileOutDirection
    {
        get => this.fileOutDirection;
        set => this.SetProperty(ref this.fileOutDirection, value);
    }

    public bool ExcludeToolOutput
    {
        get => this.excludeToolOutput;
        set => this.SetProperty(ref this.excludeToolOutput, value);
    }

    public CustomOutputPath? CustomOutputPath => this.customOutputPath;
    public DuplicationCleaner? DuplicationCleaner => this.duplicationCleaner;

    public override string ToString()
    {
        return $"{this.outputFile}";
    }

    //// --------------------------------------------------------------------------------------------
}
