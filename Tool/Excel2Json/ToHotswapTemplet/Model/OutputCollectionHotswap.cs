namespace Excel2Json.ToHotswapTemplet.Model
{
    using System.Data;
    using Excel2Json.Binding;
    using Excel2Json.ToNormalTemplet.Model;
    using static Excel2Json.Enums;

    internal sealed class OutputCollectionHotswap
    {
        private readonly List<OutputElement> elements = new();

        private OutputCollectionHotswap()
        {
        }

        #region Antlr Interface
        public string Name { get; } = string.Empty;
        public bool HasName => string.IsNullOrEmpty(this.Name) == false;
        public IEnumerable<OutputElement> Elements => this.elements;
        public string Separator { get; private set; } = Environment.NewLine;
        public string OpenBracket { get; private set; } = "{";
        public string CloseBracket { get; private set; } = "}";
        #endregion

        public static OutputCollectionHotswap? Create(IEnumerable<Column> columns, DataRow dataRow, FileOutputDirection fileOutDirection)
        {
            var collection = new OutputCollectionHotswap();
            foreach (Column column in columns)
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

            return collection;
        }

        public string GetValue(string key)
        {
            return this.elements.Find(e => e.Key == key)?.OutputValue ?? string.Empty;
        }
    }
}
