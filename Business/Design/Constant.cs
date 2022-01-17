using System;
using System.Linq;

namespace Applique.LoadTester.Business.Design
{
    public class Constant
    {
        public Constant() { }

        public Constant(string constantExpression, string value)
        {
            constantExpression = constantExpression.Split(' ')[0]; // skip constraints
            var parts = constantExpression.Split(':', StringSplitOptions.RemoveEmptyEntries);
            Overshadow = constantExpression.StartsWith(':');
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

        public string Name { get; set; }
        public bool Overshadow { get; set; }
        public string Value { get; set; }
        public string Type { get; set; } = "string";
        public string[] Conversions { get; set; } = Array.Empty<string>();
    }
}