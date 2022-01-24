using System;

namespace Applique.LoadTester.Domain.Design
{
    public class Constant
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; } = "string";
        public bool Overshadow { get; set; }
        public decimal Tolerance { get; set; }
        public string[] Conversions { get; set; } = Array.Empty<string>();
    }
}