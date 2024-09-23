namespace Binder.Model.Detail
{
    using System.Text;
    using System.Text.Json;
    using CommunityToolkit.Mvvm.ComponentModel;
    using Du.Core.Util;
    using static Binder.Model.Enums;

    public sealed class Column : ObservableObject
    {
        private string name = string.Empty;
        private Enums.DataType dataType;
        private bool nullable;
        private bool repeated;
        private OutputDirection columnOutDirection;
        private double? min;
        private double? max;

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
            if (element.TryGetDouble("min", out var value))
            {
                this.min = value;
            }

            if (element.TryGetDouble("max", out value))
            {
                this.max = value;
            }
        }

        public string Name
        {
            get => this.name;
            set => this.SetProperty(ref this.name, value);
        }

        public Enums.DataType DataType
        {
            get => this.dataType;
            set => this.SetProperty(ref this.dataType, value);
        }

        public bool Nullable
        {
            get => this.nullable;
            set => this.SetProperty(ref this.nullable, value);
        }

        public bool Repeated
        {
            get => this.repeated;
            set => this.SetProperty(ref this.repeated, value);
        }

        public OutputDirection ColumnOutDirection
        {
            get => this.columnOutDirection;
            set => this.SetProperty(ref this.columnOutDirection, value);
        }

        public double? Min
        {
            get => this.min;
            set => this.SetProperty(ref this.min, value);
        }

        public double? Max
        {
            get => this.max;
            set => this.SetProperty(ref this.max, value);
        }
    }
}
