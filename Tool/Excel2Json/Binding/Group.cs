namespace Excel2Json.Binding;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cs.Logging;
using Excel2Json.ToNormalTemplet;

internal sealed class Group : IColumnGroup
{
    public string TableName { get; set; } = string.Empty;
    public string HideWith { get; set; } = string.Empty;
    public IReadOnlyList<Column> Columns { get; set; } = Array.Empty<Column>();
    public IReadOnlyList<Group> Groups { get; set; } = Array.Empty<Group>();
    public IReadOnlyList<NumberingGroup> NumberingGroups { get; set; } = Array.Empty<NumberingGroup>();
    IEnumerable<IColumnGroup> IColumnGroup.Groups => this.Groups.Cast<IColumnGroup>();

    public bool HasColumn(string columnName)
    {
        if (this.Columns.Any(e => e.Name == columnName))
        {
            return true;
        }

        if (this.NumberingGroups.Any(e => e.HasColumn(columnName)))
        {
            return true;
        }

        return this.Groups.Any(e => e.HasColumn(columnName));
    }

    public bool GetColumn(string columnName, [MaybeNullWhen(false)] out Column result)
    {
        Column? column = this.Columns.FirstOrDefault(e => e.Name == columnName);
        if (column != null)
        {
            result = column;
            return true;
        }

        foreach (var numberingGroup in this.NumberingGroups)
        {
            if (numberingGroup.GetColumn(columnName, out result))
            {
                return true;
            }
        }

        foreach (var group in this.Groups)
        {
            if (group.GetColumn(columnName, out result))
            {
                return true;
            }
        }

        result = null;
        return false;
    }

    internal bool Initialize(Extract extract)
    {
        foreach (var column in this.Columns)
        {
            column.Initialize(extract);
            if (column.Validate() == false)
            {
                return false;
            }
        }

        if (string.IsNullOrEmpty(this.HideWith) == false)
        {
            if (this.HasColumn(this.HideWith) == false)
            {
                Log.Error($"{extract.DebugName} hideWith 값이 올바르지 않음:{this.HideWith}");
                return false;
            }
        }

        return true;
    }
}
