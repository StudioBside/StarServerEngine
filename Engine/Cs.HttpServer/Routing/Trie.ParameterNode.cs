namespace Cs.HttpServer.Routing
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Cs.Core.Util;
    using static Cs.HttpServer.Enums;

    internal sealed partial class Trie
    {
        internal sealed class ParameterNode : Node
        {
            private static readonly ParameterTraits[] Traits = new ParameterTraits[EnumUtil<ParamType>.Count];
            private readonly ParamType paramType;

            static ParameterNode()
            {
                Traits[(int)ParamType.Byte] = new ParameterTraits(
                    ParamType.Byte,
                    typeof(byte),
                    "<byte>",
                    data => byte.TryParse(data, out byte result) ? (object)result : null);

                Traits[(int)ParamType.Short] = new ParameterTraits(
                    ParamType.Short, 
                    typeof(short), 
                    "<short>",
                    data => short.TryParse(data, out short result) ? (object)result : null);

                Traits[(int)ParamType.Int] = new ParameterTraits(
                    ParamType.Int, 
                    typeof(int),
                    "<int>",
                    data => int.TryParse(data, out int result) ? (object)result : null);

                Traits[(int)ParamType.Long] = new ParameterTraits(
                    ParamType.Long,
                    typeof(long),
                    "<long>",
                    data => long.TryParse(data, out long result) ? (object)result : null);

                Traits[(int)ParamType.String] = new ParameterTraits(
                    ParamType.String, 
                    typeof(string),
                    "<string>",
                    data => data);

                Traits[(int)ParamType.Boolean] = new ParameterTraits(
                    ParamType.Boolean,
                    typeof(bool),
                    "<bool>",
                    data => bool.TryParse(data, out bool result) ? (object)result : null);

                Traits[(int)ParamType.DateTime] = new ParameterTraits(
                    ParamType.DateTime,
                    typeof(DateTime),
                    "<datetime>",
                    data => DateTime.TryParse(data, out DateTime result) ? (object)result : null);

                Traits[(int)ParamType.TimeSpan] = new ParameterTraits(
                    ParamType.TimeSpan,
                    typeof(TimeSpan),
                    "<timespan>",
                    data => TimeSpan.TryParse(data, out TimeSpan result) ? (object)result : null);
            }

            public ParameterNode(string key, ParamType paramType) : base(key)
            {
                this.paramType = paramType;
            }

            public static bool StringToParamType(string data, out ParamType result)
            {
                foreach (var trait in Traits)
                {
                    if (data == trait.UriExpression)
                    {
                        result = trait.ParamType;
                        return true;
                    }
                }

                result = ParamType.String;
                return false;
            }

            public static bool CheckParamTypeMatching(ParamType paramType, Type type)
            {
                int index = (int)paramType;
                return Traits[index].SystemType == type;
            }

            public bool TryParse(string data, [NotNullWhen(true)] out object? result)
            {
                int index = (int)this.paramType;
                result = Traits[index].Parser.Invoke(data);
                return result != null;
            }

            private readonly struct ParameterTraits
            {
                public ParameterTraits(ParamType paramType, Type systemType, string uriExpression, Func<string, object?> parser)
                {
                    this.ParamType = paramType;
                    this.UriExpression = uriExpression;
                    this.SystemType = systemType;
                    this.Parser = parser;
                }

                public ParamType ParamType { get; }
                public string UriExpression { get; }
                public Type SystemType { get; }
                public Func<string, object?> Parser { get; }
            }
        }
    }
}
