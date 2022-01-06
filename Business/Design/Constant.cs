using System;
using System.Linq;

namespace Applique.LoadTester.Business.Design
{
    public class Constant
    {
        public Constant() { }

        public Constant(string nameAndType, string value)
        {
            var parts = nameAndType.Split(':');
            Name = parts[0];
            Value = value;
            if (parts.Length == 2)
            {
                var types = parts[1];
                var typeParts = types.Split("->");
                Type = typeParts[0];
                Conversions = typeParts.Skip(1).ToArray();
            }
        }

        public const string Int = "int";
        public const string Decimal = "decimal";
        public const string DateTime = "date";
        public const string String = "string";

        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; } = "string";
        public string[] Conversions { get; private set; } = Array.Empty<string>();

        public object CreateValue()
        {
            var value = ParseValue();
            var from = Type;
            foreach (var conversion in Conversions)
            {
                value = Cast(value, from, conversion);
                from = conversion;
            }
            return value;
        }

        private static object Cast(object value, string from, string to)
            => to switch
            {
                Int when from == Decimal => decimal.ToInt32((decimal)value),
                _ => throw new InvalidOperationException($"Cannot cast {value} from {from} to {to}")
            };

        private object ParseValue()
            => Type switch
            {
                DateTime => System.DateTime.Parse(Value),
                Int => int.Parse(Value),
                Decimal => decimal.Parse(Value),
                _ => Value,
            };
    }
}