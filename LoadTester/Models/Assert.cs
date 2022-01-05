namespace LoadTester
{
    public class Assert
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public AssertResult Apply(Bindings bindings, object actualValue)
        {
            var value = bindings.SubstituteVariables(Value);
            var res = $"{actualValue}" == value;
            return res ? new AssertResult
            {
                Success = true,
                Message = $"{Name} is {actualValue} as expected"
            } 
            : new AssertResult { Message = $"{Name} is {actualValue} but expected {value}" };
        }
    }
}