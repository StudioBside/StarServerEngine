namespace Binder.Models.Detail;

using System.Collections.Generic;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using Du.Core.Util;
using static Binder.Models.Enums;

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