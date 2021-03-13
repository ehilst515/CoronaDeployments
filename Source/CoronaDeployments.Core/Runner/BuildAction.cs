using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoronaDeployments.Core.Runner
{
    public sealed class BuildAction : IRunnerAction
    {
        public BuildAction()
        {
            Log = new StringBuilder();
        }

        public StringBuilder Log { get; private set; }
        public bool HasErrors { get; private set; }

        public async Task Implementation(IRunnerActionPayload p)
        {
            LogInfo($"{nameof(BuildAction)} Started.");

            var payload = p as BuildActionPayload;
            if (payload == null)
            {
                LogError("Payload is null");
                HasErrors = true;
                return;
            }

            LogInfo($"{nameof(BuildAction)} Done.");
        }

        private void LogError(string m)
        {
            Log.AppendLine($"{DateTime.UtcNow}: Error: {m}");
        }

        private void LogInfo(string m)
        {
            Log.AppendLine($"{DateTime.UtcNow}: Info: {m}");
        }
    }

    public sealed class BuildActionPayload : IRunnerActionPayload
    {
        public Guid ProjectId { get; set; }
    }
}
