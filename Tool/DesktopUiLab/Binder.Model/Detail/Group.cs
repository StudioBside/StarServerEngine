namespace Binder.Model.Detail;

using System;
using System.Collections.Generic;
using System.Text.Json;
using Du.Core.Util;

public sealed class Group
{
    private readonly List<Column> columns = new();
    private readonly List<Group> groups = new();
    // tableName
    private string hideWith = string.Empty; // 본인의 자식 컬럼 중, 해당 컬럼에 값이 존재하지 않으면 테이블 전체를 출력하지 않는다.
    //// numberingCount

    public Group()
    {
    }

    public Group(JsonElement element)
    {
        this.hideWith = element.GetString("hideWith", string.Empty);
        element.GetArray("columns", this.columns, element => new Column(element));
        element.TryGetArray("groups", this.groups, element => new Group(element));
    }
}
