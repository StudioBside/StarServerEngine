namespace Binder.Models.Detail;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Du.Core.Util;

internal sealed class BindRoot
{
    private readonly List<Column> columns = new();
    private readonly List<Group> groups = new();
    private readonly List<NumberingGroup> numberingGroups = new();

    public BindRoot()
    {
    }

    public BindRoot(JsonElement element)
    {
        element.TryGetArray("columns", this.columns, element => new Column(element));
        element.TryGetArray("groups", this.groups, element => new Group(element));
        element.TryGetArray("numberingGroups", this.numberingGroups, element => new NumberingGroup(element));
    }
}
