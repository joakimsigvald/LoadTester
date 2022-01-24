namespace Applique.LoadTester.Environment
{
    public struct DecimalWithTolerance
    {
        public decimal? Value { get; set; }
        public decimal Tolerance { get; set; }

        public bool IsMatch(decimal? actual)
            => Value.HasValue 
            && actual.HasValue 
            && Value.Value - Tolerance <= actual.Value 
            && Value.Value + Tolerance >= actual.Value;
    }
}