namespace LoadTester
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
                Type = parts[1];
        }

        public const string Int = "int";
        public const string DateTime = "date";
        public const string String = "string";

        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; } = "string";

        public object CreateValue()
        {
            switch (Type)
            {
                case DateTime: return System.DateTime.Parse(Value);
                case Int: return int.Parse(Value);
                default: return Value;
            }
        }
    }
}