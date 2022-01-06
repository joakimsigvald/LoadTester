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
            if (value?.StartsWith("Seed(") == true)
                throw new NotImplementedException();
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
        public string Value { get; set; }
        public string Type { get; set; } = "string";
        public bool Seed { get; set; }
        public string[] Conversions { get; private set; } = Array.Empty<string>();
    }
}