namespace Binder.Models.Detail
{
    using System;
    using System.Collections.Generic;

    internal class Groups
    {
        private readonly List<Column> columns = new();
        private readonly List<Groups> groups = new();
        // tableName
        private string hideWith = string.Empty; // 본인의 자식 컬럼 중, 해당 컬럼에 값이 존재하지 않으면 테이블 전체를 출력하지 않는다.
        // numberingCount
    }
}
