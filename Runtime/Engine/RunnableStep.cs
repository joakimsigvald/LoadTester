using Applique.LoadTester.Core.Service;
using Applique.LoadTester.Domain.Design;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Applique.LoadTester.Runtime.Engine
{
    public abstract class RunnableStep<TResponse> : IRunnableStep
    {
        private TimeSpan? _delay;
        protected readonly IBindings _bindings;
        private readonly IBindings _overloads;

        public Step Blueprint { get; private set; }

        protected RunnableStep(Step step, IBindings bindings, IBindings overloads)
        {
            Blueprint = step;
            _bindings = bindings;
            _overloads = overloads;
        }

        public Task<TimeSpan> Run() => RunInTime(DoRun);

        private async Task<TimeSpan> RunInTime(Func<Task<TResponse>> run)
        {
            try
            {
                _bindings.OverloadWith(_overloads);
                var sw = Stopwatch.StartNew();
                TResponse response = await run();
                sw.Stop();
                var elapsed = sw.Elapsed;
                HandleResponse(response);
                return elapsed;
            }
            finally
            {
                _bindings.OverloadWith(null);
            }
        }

        protected abstract Task<TResponse> DoRun();

        protected virtual void HandleResponse(TResponse response) { }
        protected TimeSpan Delay => _delay ?? (_delay = TimeSpan.FromMilliseconds(Blueprint.DelayMs)).Value;
    }
}