using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CoronaDeployments.Core.Runner
{
    public class BackgroundRunner : IDisposable
    {
        private readonly Timer timer;

        public BackgroundRunner(string name, IRunnerAction impl, IRunnerActionPayload payload)
        {
            Name = name;
            ActionImplementation = impl;
            ActionImplementationParameters = payload;

            timer = new Timer(RunImpl, null, Timeout.Infinite, Timeout.Infinite);
        }

        public string Name { get; }
        public IRunnerAction ActionImplementation { get; }
        public IRunnerActionPayload ActionImplementationParameters { get; }

        public void Start()
        {
            // Start immediatly
            timer.Change(0, Timeout.Infinite);
        }

        private async void RunImpl(object state)
        {
            Log.Information($"Runner {Name} is running...");

            // Do our work here.
            await ActionImplementation.Implementation(ActionImplementationParameters);

            Log.Information($"Runner {Name} is taking a break...");

            timer.Change(1_000, Timeout.Infinite);
        }

        public void Stop()
        {
            timer?.Change(Timeout.Infinite, Timeout.Infinite);
            timer?.Dispose();
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
