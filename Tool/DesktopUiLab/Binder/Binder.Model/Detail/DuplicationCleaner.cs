namespace Binder.Model.Detail;

using System.Collections.Generic;
using System.Text.Json;
using Cs.Core.Util;
using static Binder.Model.Enums;

public sealed class DuplicationCleaner
{
    private readonly OutputDirection fileOutDirection = OutputDirection.Client;
    private readonly List<string> columnNames = new();

    public DuplicationCleaner(JsonElement element)
    {
        this.fileOutDirection = element.GetEnum<OutputDirection>("fileOutDirection");
        element.GetArray("columnNames", this.columnNames);
    }

    internal OutputDirection FileOutDirection => this.fileOutDirection;
    internal IList<string> ColumnNames => this.columnNames;
}