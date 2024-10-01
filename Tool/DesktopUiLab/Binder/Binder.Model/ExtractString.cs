namespace Binder.Model;

using System.Collections.Generic;
using System.Text.Json;
using Binder.Model.Detail;
using CommunityToolkit.Mvvm.ComponentModel;
using Cs.Core.Util;

public sealed class ExtractString : ObservableObject
{
    private readonly List<Source> sources = new();
    private string outputFile = string.Empty;
    private string idColumnName = string.Empty;
    private string valueColumnName = string.Empty;

    public ExtractString(JsonElement element)
    {
        this.outputFile = element.GetString("outputFile");
        element.GetArray("sources", this.sources, element => new Source(element));
        this.idColumnName = element.GetString("IdColumnName");
        this.valueColumnName = element.GetString("ValueColumnName");
    }

    public override string ToString()
    {
        return $"{this.outputFile}";
    }

    //// --------------------------------------------------------------------------------------------
}
