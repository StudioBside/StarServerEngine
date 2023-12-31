namespace Excel2Json;

internal sealed class Enums
{
    public enum DataType
    {
        Bool,
        Int8,
        Int16,
        Int32,
        Int64,
        Uint8,
        Uint16,
        Uint32,
        Uint64,
        String,
        Float,
        Double,
    }

    public enum ColumnOutputDirection
    {
        All,
        Server,
        Client,
    }

    public enum FileOutputDirection
    {
        All,
        Server,
        Client,
        Tool,
    }

    public enum FileWritingResult
    {
        AlreadyOpened,
        NotChanged,
        Changed,
        Error,
        Skip,
        Remove,
    }

    public enum ExcelEnumColumn
    {
        EnumTypeName,
        EnumBaseType,
        EnumLiteral,
        EnumValue,
        DescriptionKor,
        DescriptionEng,
    }

    public enum StringTableColumn
    {
        StringId,
        StringValue, // 나중에는 Kor이 되어야 할 듯.
    }

    public enum SystemColumn
    {
        IsStable,
        ContentsTag,
    }
}
