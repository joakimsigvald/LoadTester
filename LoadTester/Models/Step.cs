using Newtonsoft.Json.Linq;
using System;

namespace LoadTester
{
    public class Step
    {
        public string Endpoint { get; set; }
        public string Args { get; set; }
        public dynamic Body { get; set; }
        public JObject Response { get; set; }
        public int DelayMs { get; set; }
        public int Times { get; set; }
        public TimeSpan? _delay;
        public TimeSpan Delay => _delay ?? (_delay = TimeSpan.FromMilliseconds(DelayMs)).Value;
        public bool AbortOnSuccess { get; set; }
        public bool AbortOnFail { get; set; } = true;
    }
}