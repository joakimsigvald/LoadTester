using System;
using Applique.LoadTester.Domain.Assembly;

namespace Applique.LoadTester.Logic.Runtime.Result
{
    public class StepDuration
    {
        public IStep Step { get; set; }
        public TimeSpan Duration { get; set; }
    }
}