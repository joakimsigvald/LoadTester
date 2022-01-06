using Newtonsoft.Json.Linq;
using System;

namespace Applique.LoadTester.Business.Design
{
    public class Step
    {
        public string Endpoint { get; set; }
        public string Args { get; set; } = string.Empty;
        public dynamic Body { get; set; }
        public JObject Response { get; set; }
        public int DelayMs { get; set; } = 0;
        public int Times { get; set; } = 1;
        public TimeSpan? _delay;
        public TimeSpan Delay => _delay ?? (_delay = TimeSpan.FromMilliseconds(DelayMs)).Value;
        public bool BreakOnSuccess { get; set; }
        public bool RetryOnFail { get; set; }
    }
}