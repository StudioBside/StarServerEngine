namespace Excel2Json.Binding;

using System;
using System.Runtime.Serialization;
using Cs.Logging;
using static Excel2Json.Enums;

internal sealed class Column
{
    public const string IsStableColumnName = "IsStable";

    #region Antlr Interface
    public string Name { get; set; } = string.Empty;
    public string CSDataType => this.DataType switch
    {
        DataType.Bool => "bool",
        DataType.Int8 => "sbyte",
        DataType.Int16 => "short",
        DataType.Int32 => "int",
        DataType.Int64 => "long",
        DataType.Uint8 => "byte",
        DataType.Uint16 => "ushort",
        DataType.Uint32 => "uint",
        DataType.Uint64 => "ulong",
        DataType.Float => "float",
        DataType.Double => "double",
        DataType.String => "string",
        _ => "string",
    };
    public string? CSDefaultValue => this.DataType switch
    {
        DataType.String => "string.Empty",
        _ => null,
    };
    #endregion

    public DataType DataType { get; set; }
    public ColumnOutputDirection ColumnOutDirection { get; set; }
    public bool Nullable { get; set; }
    public bool Repeated { get; set; }
    public double Min { get; set; } = double.MinValue;
    public double Max { get; set; } = double.MaxValue;
    public bool NeedQuotationMark => this.DataType == DataType.String;
    public string DebugName => $"[{this.Extract?.OutputFile} 컬럼명:{this.Name} 타입:{this.DataType}]";
    public IExtract Extract { get; private set; } = null!;

    public void Initialize(IExtract extract)
    {
        this.Extract = extract;
    }

    public bool Validate()
    {
        var extract = this.Extract;
        if ((extract.FileOutDirection == FileOutputDirection.Server && this.ColumnOutDirection == ColumnOutputDirection.Client)
            || (extract.FileOutDirection == FileOutputDirection.Client && this.ColumnOutDirection == ColumnOutputDirection.Server))
        {
            Log.Error($"{extract.DebugName} 파일 출력방향 {extract.FileOutDirection}과 컬럼 출력방향 {this.ColumnOutDirection}의 관계가 올바르지 않습니다. 컬럼명:{this.Name}");
            return false;
        }

        return true;
    }

    public bool NeedToWrite(FileOutputDirection fileOutDirection)
    {
        if (this.ColumnOutDirection == ColumnOutputDirection.All)
        {
            return true;
        }

        return fileOutDirection switch
        {
            FileOutputDirection.All => true,
            FileOutputDirection.Tool => true,
            FileOutputDirection.Server => this.ColumnOutDirection == ColumnOutputDirection.Server,
            FileOutputDirection.Client => this.ColumnOutDirection == ColumnOutputDirection.Client,
            _ => throw new Exception($"unknown direction type:{fileOutDirection}"),
        };
    }

    public bool ValidateDataType(string value)
    {
        return this.DataType switch
        {
            DataType.Bool => bool.TryParse(value, out _),
            DataType.Int8 => byte.TryParse(value, out _),
            DataType.Int16 => short.TryParse(value, out _),
            DataType.Int32 => int.TryParse(value, out _),
            DataType.Int64 => long.TryParse(value, out _),
            DataType.Uint8 => char.TryParse(value, out _),
            DataType.Uint16 => ushort.TryParse(value, out _),
            DataType.Uint32 => uint.TryParse(value, out _),
            DataType.Uint64 => ulong.TryParse(value, out _),
            DataType.Float => float.TryParse(value, out _),
            DataType.Double => double.TryParse(value, out _),
            DataType.String => true, // 모두 다 허용.
            _ => throw new NotImplementedException($"invlid SupportType:{this.DataType}"),
        };
    }

    public bool ValidateRange(string value)
    {
        bool typeNeedToCheckRange = this.DataType switch
        {
            DataType.Int8 => true,
            DataType.Int16 => true,
            DataType.Int32 => true,
            DataType.Int64 => true,
            DataType.Uint8 => true,
            DataType.Uint16 => true,
            DataType.Uint32 => true,
            DataType.Uint64 => true,
            DataType.Float => true,
            DataType.Double => true,
            _ => false,
        };

        if (typeNeedToCheckRange == false)
        {
            return true;
        }

        var valueDouble = double.Parse(value);
        return this.Min <= valueDouble && valueDouble <= this.Max;
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        if (this.Name.Equals(nameof(SystemColumn.IsStable)) ||
            this.Name.Equals(nameof(SystemColumn.ContentsTag)))
        {
            ErrorContainer.Add($"시스템에 예약되어 사용 불가한 컬럼명 입니다. columnName:{this.Name}");
        }
    }
}
