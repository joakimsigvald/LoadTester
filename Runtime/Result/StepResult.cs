using Applique.LoadTester.Design;
using System;
using System.Linq;

namespace Applique.LoadTester.Runtime.Result
{
    public class StepResult
    {
        public StepResult(Step step, TimeSpan[] orderedDurations)
        {
            Step = step;
            Max = orderedDurations.Last();
            Min = orderedDurations.First();
            Mean = GetQuantile(orderedDurations, 0.5f);
            Q75 = GetQuantile(orderedDurations, 0.75f);
            Q90 = GetQuantile(orderedDurations, 0.9f);
        }

        private static TimeSpan GetQuantile(TimeSpan[] orderedDurations, float quantile)
            => orderedDurations.Skip((int)(orderedDurations.Length * quantile)).First();

        public Step Step { get; private set; }
        public TimeSpan Min { get; private set; }
        public TimeSpan Max { get; private set; }
        public TimeSpan Mean { get; private set; }
        public TimeSpan Q75 { get; private set; }
        public TimeSpan Q90 { get; private set; }
    }
}