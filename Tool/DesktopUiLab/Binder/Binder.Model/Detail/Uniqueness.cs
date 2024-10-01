namespace Binder.Model.Detail;

using System.Collections.Generic;
using System.Text.Json;
using Cs.Core.Util;

public sealed class Uniqueness
{
    private readonly string name = string.Empty;
    private readonly List<string> columnNames = new();

    public Uniqueness(JsonElement element)
    {
        this.name = element.GetString("name");
        element.GetArray("columnNames", this.columnNames);
    }

    public string Name => this.name;
    public IList<string> ColumnNames => this.columnNames;
}