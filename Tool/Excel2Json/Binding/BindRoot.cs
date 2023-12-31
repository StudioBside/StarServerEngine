namespace Excel2Json.Binding;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Excel2Json.ToNormalTemplet;

using static Excel2Json.Enums;

internal sealed class BindRoot : IColumnGroup
{
    public string TableName { get; } = string.Empty;
    public IReadOnlyList<Column> Columns { get; set; } = Array.Empty<Column>();
    public IReadOnlyList<Group> Groups { get; set; } = Array.Empty<Group>();
    public IReadOnlyList<NumberingGroup> NumberingGroups { get; set; } = Array.Empty<NumberingGroup>();
    IEnumerable<IColumnGroup> IColumnGroup.Groups => this.Groups.Cast<IColumnGroup>();

    // 아직 데이터와는 무관하고, bind 정의 자체의 정합성 확인만 진행한다.
    public bool Initialize(Extract extract)
    {
        foreach (var column in this.Columns)
        {
            column.Initialize(extract);
            if (column.Validate() == false)
            {
                return false;
            }
        }

        foreach (var group in this.Groups)
        {
            if (group.Initialize(extract) == false)
            {
                return false;
            }
        }

        return true;
    }

    public bool HasColumn(string name)
    {
        return this.Columns.Any(e => e.Name == name) ||
            this.NumberingGroups.Any(e => e.HasColumn(name)) ||
            this.Groups.Any(e => e.HasColumn(name));
    }

    public bool GetColumn(string name, [MaybeNullWhen(false)] out Column result)
    {
        Column? column = this.Columns.FirstOrDefault(e => e.Name == name);
        if (column != null)
        {
            result = column;
            return true;
        }

        foreach (var numberingGroup in this.NumberingGroups)
        {
            if (numberingGroup.GetColumn(name, out result))
            {
                return true;
            }
        }

        foreach (var group in this.Groups)
        {
            if (group.GetColumn(name, out result))
            {
                return true;
            }
        }

        result = null;
        return false;
    }

    public IEnumerable<Column> GetAllColumns()
    {
        return this.Columns.Concat(this.Groups.SelectMany(e => e.Columns));
    }

    public void AddSystemColumn(SystemColumn systemColumn)
    {
        var newColumns = new List<Column>
        {
            new Column
            {
                Name = systemColumn.ToString(),
                DataType = DataType.String,
                Nullable = false,
            },
        };

        newColumns.AddRange(this.Columns);
        this.Columns = newColumns;
    }
}
