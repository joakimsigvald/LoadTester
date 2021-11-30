namespace LoadTester
{
    public class Assert
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public AssertResult Apply(object actualValue)
            => $"{actualValue}" == Value ? new AssertResult
            {
                Success = true,
                Message = $"{Name} is {actualValue} as expected"
            }
            : new AssertResult { Message = $"{Name} is {actualValue} but expected {Value}" };
    }
}