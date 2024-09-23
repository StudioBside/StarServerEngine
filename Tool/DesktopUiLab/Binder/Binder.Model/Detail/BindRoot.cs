namespace Binder.Model.Detail;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using Du.Core.Util;

public sealed class BindRoot
{
    private readonly ObservableCollection<Column> columns = new();
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

    public IList<Column> Columns => this.columns;
    public IList<Group> Groups => this.groups;
    public IList<NumberingGroup> NumberingGroups => this.numberingGroups;
}
