namespace Binder.Model.Detail;

using System.Collections.Generic;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using Du.Core.Util;
using static Binder.Model.Enums;

public sealed class NumberingGroup : ObservableObject
{
    private readonly List<Column> columns = new();
    private string tableName = string.Empty;
    private int numberingCount; // 컬럼명 뒤에 '_1' 부터 시작하는 숫자가 붙어 동일한 패턴으로 반복되는 횟수를 지정한다.
    private string hideWith = string.Empty; // 본인의 자식 컬럼 중, 해당 컬럼에 값이 존재하지 않으면 테이블 전체를 출력하지 않는다.
    private OutputDirection columnOutDirection;

    public NumberingGroup()
    {
    }

    public NumberingGroup(JsonElement element)
    {
        this.numberingCount = element.GetInt32("numberingCount");
        this.hideWith = element.GetString("hideWith", string.Empty);
        this.tableName = element.GetString("tableName", string.Empty);
        this.columnOutDirection = element.GetEnum("columnOutDirection", OutputDirection.All);
        element.GetArray("columns", this.columns, element => new Column(element));
    }

    public string TableName
    {
        get => this.tableName;
        set => this.SetProperty(ref this.tableName, value);
    }

    public int NumberingCount
    {
        get => this.numberingCount;
        set => this.SetProperty(ref this.numberingCount, value);
    }

    public string HideWith
    {
        get => this.hideWith;
        set => this.SetProperty(ref this.hideWith, value);
    }

    public OutputDirection ColumnOutDirection
    {
        get => this.columnOutDirection;
        set => this.SetProperty(ref this.columnOutDirection, value);
    }
}
