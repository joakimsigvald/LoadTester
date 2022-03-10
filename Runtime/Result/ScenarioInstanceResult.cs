using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Logic.Runtime.Engine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Applique.LoadTester.Logic.Runtime.Result;

public class ScenarioInstanceResult
{
    public static ScenarioInstanceResult Succeeded(
        RunnableScenario scenario, TimeSpan duration, IList<StepDuration> stepDurations)
        => new()
        {
            Success = true,
            Duration = duration,
            StepDurations = stepDurations.ToArray(),
            Bindings = scenario.Bindings
        };

    public static ScenarioInstanceResult Failed(RunnableScenario scenario, string error) => new()
    {
        Error = error,
        Bindings = scenario.Bindings
    };

    public bool Success { get; set; }
    public TimeSpan Duration { get; set; }
    public StepDuration[] StepDurations { get; set; }
    public string Error { get; set; }
    public IBindings Bindings { get; private set; }
}