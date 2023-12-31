namespace Excel2Json.ToEnum.Model;

using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Excel2Json;
using Excel2Json.ToEnum;
using static Excel2Json.Enums;

// enum 타입 정의 하나에 해당하는 타입.
internal sealed class EnumSetModel
{
    private readonly ExtractEnum record;
    private readonly List<EnumElement> enumElements = new();
    private string enumBaseType = string.Empty;

    public EnumSetModel(ExtractEnum record, string enumTypeName)
    {
        this.record = record;
        this.EnumTypeName = enumTypeName;
    }

    #region Antlr Interface
    public string EnumTypeName { get; private set; }
    public IReadOnlyList<EnumElement> EnumElements => this.enumElements;
    public bool HasBaseType => string.IsNullOrEmpty(this.enumBaseType) == false;
    public string EnumBaseType => this.enumBaseType;
    #endregion

    public void Load(DataRow dataRow)
    {
        if (this.enumElements.Any() == false)
        {
            // 처음 로딩하는 경우는 공통 속성들도 함께 읽어들인다.
            this.enumBaseType = dataRow.GetString(ExcelEnumColumn.EnumBaseType);
        }

        var literal = dataRow.GetString(ExcelEnumColumn.EnumLiteral);
        var enumValueStr = dataRow.GetString(ExcelEnumColumn.EnumValue);
        var descriptionKor = dataRow.GetString(ExcelEnumColumn.DescriptionKor);
        var descriptionEng = dataRow.GetString(ExcelEnumColumn.DescriptionEng);
        int? enumValue = string.IsNullOrEmpty(enumValueStr) ? null : int.Parse(enumValueStr);
        this.enumElements.Add(new(literal, enumValue, descriptionKor, descriptionEng));
    }

    public sealed record EnumElement(
        string EnumLiteral,
        int? EnumValue,
        string DescriptionKor,
        string DescriptionEng)
    {
        public bool HasValue => this.EnumValue.HasValue;
        public int Value => this.EnumValue.HasValue ? this.EnumValue.Value : 0;
        public bool HasDescriptionKor => string.IsNullOrEmpty(this.DescriptionKor) == false;
    }
}
