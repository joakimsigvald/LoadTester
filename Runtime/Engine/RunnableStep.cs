using Applique.LoadTester.Domain.Design;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Applique.LoadTester.Runtime.Engine
{
    public abstract class RunnableStep<TResponse> : IRunnableStep
    {
        private TimeSpan? _delay;

        public Step Blueprint { get; private set; }

        protected RunnableStep(Step step) => Blueprint = step;

        public Task<TimeSpan> Run() => RunInTime(DoRun);

        private async Task<TimeSpan> RunInTime(Func<Task<TResponse>> run)
        {
            var sw = Stopwatch.StartNew();
            TResponse response = await run();
            sw.Stop();
            var elapsed = sw.Elapsed;
            await HandleResponse(response);
            return elapsed;
        }

        protected abstract Task<TResponse> DoRun();

        protected abstract Task HandleResponse(TResponse response);
        protected TimeSpan Delay => _delay ?? (_delay = TimeSpan.FromMilliseconds(Blueprint.DelayMs)).Value;
    }
}