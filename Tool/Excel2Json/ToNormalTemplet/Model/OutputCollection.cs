namespace Excel2Json.ToNormalTemplet.Model;

using System.Data;
using Excel2Json;
using Excel2Json.Binding;
using static Excel2Json.Enums;

// 출력하는 데이터의 1 row에 해당하는 타입.
internal sealed class OutputCollection
{
    private readonly List<OutputElement> elements = new();
    private readonly List<OutputCollection> subCollections = new();

    private OutputCollection(string tableName)
    {
        this.Name = tableName;
    }

    #region Antlr Interface
    public string Name { get; }
    public bool HasName => string.IsNullOrEmpty(this.Name) == false;
    public IEnumerable<OutputElement> Elements => this.elements;
    public IEnumerable<OutputCollection> SubTables => this.subCollections;
    public string Separator { get; private set; } = Environment.NewLine;
    public string OpenBracket { get; private set; } = "{";
    public string CloseBracket { get; private set; } = "}";
    #endregion

    public static OutputCollection? Create(IColumnGroup group, DataRow dataRow, FileOutputDirection fileOutDirection)
    {
        var collection = new OutputCollection(group.TableName);
        foreach (Column column in group.Columns)
        {
            string value = dataRow.GetString(column.Name);
            var element = OutputElement.Create(column, value);
            if (element == null)
            {
                return null;
            }

            if (column.Nullable && element.HasValue == false)
            {
                continue;
            }

            if (column.NeedToWrite(fileOutDirection) == false)
            {
                continue;
            }

            collection.elements.Add(element);
        }

        foreach (var numberingGroup in group.NumberingGroups)
        {
            if (numberingGroup.CheckDirection(fileOutDirection) == false)
            {
                continue;
            }

            var subCollection = CreateNumberingGroup(numberingGroup, dataRow);
            if (subCollection == null)
            {
                return null;
            }

            if (subCollection.subCollections.Any() == false)
            {
                continue;
            }

            collection.subCollections.Add(subCollection);
        }

        foreach (Group subGroup in group.Groups)
        {
            if (string.IsNullOrEmpty(subGroup.HideWith) == false)
            {
                string value = dataRow.GetString(subGroup.HideWith);
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }
            }

            var subCollection = Create(subGroup, dataRow, fileOutDirection);
            if (subCollection == null)
            {
                return null;
            }

            collection.subCollections.Add(subCollection);
        }

        if (collection.elements.Count <= 0 && collection.subCollections.Count <= 0)
        {
            return null;
        }

        return collection;
    }

    public static OutputCollection? CreateNumberingGroup(NumberingGroup group, DataRow dataRow)
    {
        var result = new OutputCollection(group.TableName);
        result.OpenBracket = "[";
        result.CloseBracket = "]";

        for (int i = 0; i < group.NumberingCount; ++i)
        {
            int number = i + 1;
            var collection = new OutputCollection(tableName: string.Empty);
            foreach (Column column in group.Columns)
            {
                var columnName = $"{column.Name}{number}";
                string value = dataRow.GetString(columnName);

                if (string.IsNullOrWhiteSpace(group.HideWith) == false &&
                    column.Name.Equals(group.HideWith) &&
                    string.IsNullOrEmpty(value))
                {
                    collection.elements.Clear();
                    break;
                }

                var element = OutputElement.Create(column, value);
                if (element == null)
                {
                    return null;
                }

                if (column.Nullable && element.HasValue == false)
                {
                    continue;
                }

                collection.elements.Add(element);
            }

            // 비어있는 테이블은 담지 않는다.
            if (collection.elements.Any() == false)
            {
                continue;
            }

            result.subCollections.Add(collection);
        }

        return result;
    }

    public string GetValue(string key)
    {
        return this.elements.Find(e => e.Key == key)?.OutputValue ?? string.Empty;
    }
}
