using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoronaDeployments.Core.Runner
{
    public class Runner
    {
        private readonly Timer timer;

        public Runner(string name, IRunnerAction impl, IRunnerActionPayload payload)
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
            //timer.Change();
        }

        private async void RunImpl(object state)
        {
            
        }
    }

    public interface IRunnerAction
    {
        public Task Implementation(IRunnerActionPayload payload);
    }

    public interface IRunnerActionPayload
    {
    }
}
