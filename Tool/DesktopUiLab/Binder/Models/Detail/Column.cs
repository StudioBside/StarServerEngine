namespace Binder.Models.Detail
{
    using System.Text;
    using System.Text.Json;
    using Du.Core.Util;
    using static Binder.Models.Enums;

    internal sealed class Column
    {
        private string name = string.Empty;
        private Enums.DataType dataType;
        private bool nullable;
        private bool repeated;
        private OutputDirection columnOutDirection;
        private string min = string.Empty;
        private string max = string.Empty;

        public Column()
        {
        }

        public Column(JsonElement element)
        {
            this.name = element.GetString("name");
            this.dataType = element.GetEnum<DataType>("dataType");
            this.nullable = element.GetBoolean("nullable", false);
            this.repeated = element.GetBoolean("repeated", false);
            this.columnOutDirection = element.GetEnum("columnOutDirection", OutputDirection.All);
            this.min = element.GetString("min", string.Empty);
            this.max = element.GetString("max", string.Empty);
        }

        public string Name { get => this.name; set => this.name = value; }
        public Enums.DataType DataType { get => this.dataType; set => this.dataType = value; }
    }
}
