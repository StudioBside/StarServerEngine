namespace Excel2Json.Binding;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Cs.Logging;
using Excel2Json.ToNormalTemplet;
using static Excel2Json.Enums;

internal sealed class NumberingGroup
{
    public string TableName { get; set; } = string.Empty;
    public string HideWith { get; set; } = string.Empty;
    public int NumberingCount { get; set; }
    public ColumnOutputDirection ColumnOutDirection { get; set; }
    public IReadOnlyList<Column> Columns { get; set; } = Array.Empty<Column>();
    public Extract Extract { get; private set; } = null!;

    public bool CheckDirection(FileOutputDirection fileOutDirection)
    {
        if (fileOutDirection == FileOutputDirection.Tool)
        {
            return true;
        }

        if (this.ColumnOutDirection == ColumnOutputDirection.All)
        {
            return true;
        }

        return (fileOutDirection == FileOutputDirection.Server && this.ColumnOutDirection == ColumnOutputDirection.Server) ||
           (fileOutDirection == FileOutputDirection.Client && this.ColumnOutDirection == ColumnOutputDirection.Client);
    }

    public bool HasColumn(string columnName)
    {
        if (this.Columns.Any(e => e.Name == columnName))
        {
            return true;
        }

        if (TryParseColumnNamePattern(columnName, out var parsedName))
        {
            return this.HasColumn(parsedName);
        }

        return false;
    }

    public bool GetColumn(string columnName, [MaybeNullWhen(false)] out Column result)
    {
        Column? column = this.Columns.FirstOrDefault(e => e.Name == columnName);
        if (column != null)
        {
            result = column;
            return true;
        }

        if (TryParseColumnNamePattern(columnName, out var parsedName))
        {
            return this.GetColumn(parsedName, out result);
        }

        result = null;
        return false;
    }

    internal bool Initialize(Extract extract)
    {
        this.Extract = extract;

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

    // -----------------------------------------------------------------------------------------------
    private static bool TryParseColumnNamePattern(string columnName, out string parsedName)
    {
        var match = Regex.Match(columnName, @"(.+)\d+");
        if (match.Success == false)
        {
            parsedName = string.Empty;
            return false;
        }

        parsedName = match.Groups[1].Value;
        return true;
    }
}
