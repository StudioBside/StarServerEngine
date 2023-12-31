namespace Excel2Json.ToNormalTemplet.Model;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Cs.Logging;
using Excel2Json.Binding;
using static Excel2Json.Enums;

// excel의 컬럼 하나에 대응하는 타입. 컬럼의 속성에 따라 값은 여러개가 될 수 있다.
internal sealed class OutputElement
{
    private readonly List<string> values = new();
    private readonly string openBracket = string.Empty;
    private readonly string closeBracket = string.Empty;

    private OutputElement(Column column)
    {
        this.Column = column;
        if (column.Repeated)
        {
            this.openBracket = "[";
            this.closeBracket = "]";
        }
    }

    public bool HasValue => this.values.Any();

    public Column Column { get; }

    #region Antlr Interface
    public string Key => this.Column.Name;
    public string OutputValue => $"{this.openBracket}{string.Join(", ", this.values)}{this.closeBracket}";
    #endregion

    public static OutputElement? Create(Column column, string rawData)
    {
        var result = new OutputElement(column);
        if (string.IsNullOrWhiteSpace(rawData))
        {
            if (column.Nullable == false)
            {
                ErrorContainer.Add($"{column.DebugName} nullable이 아닌데 값이 비어있음");
                return null;
            }

            return result; // null이 허용된 스키마면서 값이 없는 경우.
        }

        if (column.Repeated)
        {
            result.values.AddRange(rawData.Split(',', StringSplitOptions.TrimEntries));
        }
        else
        {
            result.values.Add(rawData);
        }

        foreach (string value in result.values)
        {
            if (!column.ValidateDataType(value))
            {
                ErrorContainer.Add($"{column.DebugName} 형식에 맞지 않는 값입니다. value:{value}");
                return null;
            }

            if (!column.ValidateRange(value))
            {
                ErrorContainer.Add($"{column.DebugName} 값의 범위가 올바르지 않습니다. value:{value} range:[{column.Min}, {column.Max}");
                return null;
            }
        }

        // 따옴표를 붙여야 하는 타입이면 여기에서 미리 붙여둔다.
        if (column.NeedQuotationMark)
        {
            for (int i = 0; i < result.values.Count; i++)
            {
                result.values[i] = $"\"{result.values[i]}\"";
            }
        }

        // 엑셀에서 boolean은 대문자로 자동치환된다. lua는 소문자를 인식하니까.. bool은 소문자로 변경 저장하자.
        if (column.DataType == DataType.Bool)
        {
            for (int i = 0; i < result.values.Count; i++)
            {
                result.values[i] = result.values[i].ToLower();
            }
        }

        // 문자열에 이스케이프가 필요하면 여기서 붙인다.
        if (column.DataType == DataType.String)
        {
            for (int i = 0; i < result.values.Count; i++)
            {
                result.values[i] = $"\"{Regex.Replace(result.values[i][1..^1], Regex.Escape("\""), "\\\"")}\"";
            }
        }

        return result;
    }
}
