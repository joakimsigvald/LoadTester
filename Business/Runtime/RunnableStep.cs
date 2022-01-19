﻿using Applique.LoadTester.Business.Design;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Applique.LoadTester.Business.Runtime
{
    public abstract class RunnableStep<TResponse> : IRunnableStep
    {
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
    }
}