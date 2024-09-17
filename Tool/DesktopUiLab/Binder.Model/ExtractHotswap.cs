namespace Binder.Model;

using System.Collections.Generic;
using System.Text.Json;
using Binder.Model.Detail;
using CommunityToolkit.Mvvm.ComponentModel;
using Du.Core.Util;

public sealed class ExtractHotswap : ObservableObject
{
    private readonly List<Source> sources = new();
    private readonly List<Uniqueness> uniquenesses = new();
    private string outputFile = string.Empty;
    private string keyColumn = string.Empty;
    //// columns

    public ExtractHotswap(JsonElement element)
    {
        this.outputFile = element.GetString("outputFile");
        element.GetArray("sources", this.sources, element => new Source(element));
        element.GetArray("uniquenesses", this.uniquenesses, element => new Uniqueness(element));
        this.keyColumn = element.GetString("keyColumn");
    }

    public override string ToString()
    {
        return $"{this.outputFile}";
    }

    //// --------------------------------------------------------------------------------------------
}
