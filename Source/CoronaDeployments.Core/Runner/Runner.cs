using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoronaDeployments.Core.Runner
{
    public class Runner
    {
        public Runner(string name, IRunnerAction impl, IRunnerActionPayload payload)
        {
            Name = name;
            ActionImplementation = impl;
            ActionImplementationParameters = payload;
        }

        public string Name { get; }
        public IRunnerAction ActionImplementation { get; }
        public IRunnerActionPayload ActionImplementationParameters { get; }
    }

    public interface IRunnerAction
    {
        public Task Implementation(IRunnerActionPayload payload);
    }

    public interface IRunnerActionPayload
    {
    }
}
