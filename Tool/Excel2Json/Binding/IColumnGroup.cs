namespace Excel2Json.Binding;

using System.Collections.Generic;

internal interface IColumnGroup
{
    string TableName { get; }
    IReadOnlyList<Column> Columns { get; }
    IEnumerable<IColumnGroup> Groups { get; }
    IReadOnlyList<NumberingGroup> NumberingGroups { get; }
}
