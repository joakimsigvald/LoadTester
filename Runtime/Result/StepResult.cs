using Applique.LoadTester.Core.Result;
using System;
using System.Linq;

namespace Applique.LoadTester.Logic.Runtime.Result
{
    public class StepResult : IStepResult
    {
        public StepResult(string endpoint, TimeSpan[] orderedDurations)
        {
            Endpoint = endpoint;
            Max = orderedDurations.Last();
            Min = orderedDurations.First();
            Mean = GetQuantile(orderedDurations, 0.5f);
            Q75 = GetQuantile(orderedDurations, 0.75f);
            Q90 = GetQuantile(orderedDurations, 0.9f);
        }

        private static TimeSpan GetQuantile(TimeSpan[] orderedDurations, float quantile)
            => orderedDurations.Skip((int)(orderedDurations.Length * quantile)).First();

        public string Endpoint { get; private set; }
        public TimeSpan Min { get; private set; }
        public TimeSpan Max { get; private set; }
        public TimeSpan Mean { get; private set; }
        public TimeSpan Q75 { get; private set; }
        public TimeSpan Q90 { get; private set; }
    }
}