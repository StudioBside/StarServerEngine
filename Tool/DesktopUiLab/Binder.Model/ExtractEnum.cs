namespace Binder.Model;

using System.Collections.Generic;
using System.Text.Json;
using Binder.Model.Detail;
using CommunityToolkit.Mvvm.ComponentModel;
using Du.Core.Util;
using static Binder.Model.Enums;

public sealed class ExtractEnum : ObservableObject
{
    private readonly List<Source> sources = new();
    private string outputFile = string.Empty;
    private OutputDirection fileOutDirection = OutputDirection.All;
    private string codeNamespace = string.Empty;

    public ExtractEnum(JsonElement element)
    {
        this.outputFile = element.GetString("outputFile");
        element.GetArray("sources", this.sources, element => new Source(element));
        this.fileOutDirection = element.GetEnum<OutputDirection>("fileOutDirection");
        this.codeNamespace = element.GetString("namespace");
    }

    public override string ToString()
    {
        return $"{this.outputFile}";
    }

    //// --------------------------------------------------------------------------------------------
}
