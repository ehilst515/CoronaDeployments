using CoronaDeployments.Core.Models;
using CoronaDeployments.Core.Repositories;
using System;
using System.Linq;
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

        private BuildActionPayload Payload { get; set; }

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
            else
            {
                Payload = payload;
            }

            try
            {
                var requests = await payload.ProjectRepository.GetBuildAndDeployRequests(payload.ProjectId, BuildAndDeployRequestState.Created, true);
                if (requests != null && requests.Count > 0)
                {
                    // Clear the log.
                    Log.Clear();

                    foreach (var r in requests.OrderByDescending(x => x.Request.CreatedAtUtc))
                    {
                        var task = DoWork(r);

                        while (task.IsCompleted == false)
                        {
                            // Persist Log
                            await payload.ProjectRepository.UpdateBuildAndDeployRequest(r.Request.Id, Log.ToString(), null, DateTime.UtcNow);

                            // Sleep for a while
                            await Task.Delay(TimeSpan.FromSeconds(5));
                        }

                        // Mark this request as Completed
                        await payload.ProjectRepository.UpdateBuildAndDeployRequest(r.Request.Id, Log.ToString(), BuildAndDeployRequestState.Completed, null);

                        // Clear the Log.
                        Log.Clear();
                    }
                }
            }
            catch (Exception exp)
            {
                LogError(exp);
            }

            LogInfo($"{nameof(BuildAction)} Done.");
        }

        public async Task DoWork(BuildAndDeployRequestModel request)
        {
            try
            {
                LogInfo("Doing work 0..."); await Task.Delay(5_000);
                LogInfo("Doing work 1..."); await Task.Delay(5_000);
                LogInfo("Doing work 2..."); await Task.Delay(5_000);
                LogInfo("Doing work 3..."); await Task.Delay(5_000);
            }
            catch (Exception exp)
            {
                LogError(exp);
            }
        }

        private void LogError(string m)
        {
            Log.AppendLine($"{DateTime.UtcNow}: Error: {m}");
        }

        private void LogError(Exception e)
        {
            Log.AppendLine($"{DateTime.UtcNow}: Error: {e}");
        }

        private void LogInfo(string m)
        {
            Log.AppendLine($"{DateTime.UtcNow}: Info: {m}");
        }
    }

    public sealed class BuildActionPayload : IRunnerActionPayload
    {
        public Guid ProjectId { get; set; }

        public IProjectRepository ProjectRepository { get; set; }

        public IServiceProvider ServiceProvider { get; set; }
    }
}