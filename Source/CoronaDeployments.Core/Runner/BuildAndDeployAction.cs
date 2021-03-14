using CoronaDeployments.Core.Models;
using CoronaDeployments.Core.Repositories;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoronaDeployments.Core.Runner
{
    public sealed class BuildAndDeployAction : IRunnerAction
    {
        public BuildAndDeployAction()
        {
            Logger = new CustomLogger();
        }

        public CustomLogger Logger { get; private set; }
        public bool HasErrors { get; private set; }

        private BuildAndDeployActionPayload Payload { get; set; }

        public async Task Implementation(IRunnerActionPayload p)
        {
            Logger.Information($"{nameof(BuildAndDeployAction)} Started.");

            var payload = p as BuildAndDeployActionPayload;
            if (payload == null)
            {
                Logger.Error("Payload is null");
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
                    Logger.Clear();

                    foreach (var r in requests.OrderByDescending(x => x.Request.CreatedAtUtc))
                    {
                        var task = DoWork(r);

                        while (task.IsCompleted == false)
                        {
                            // Persist Log
                            await payload.ProjectRepository.UpdateBuildAndDeployRequest(r.Request.Id, Logger.ToString(), null, DateTime.UtcNow);

                            // Sleep for a while
                            await Task.Delay(TimeSpan.FromSeconds(5));
                        }

                        // Mark this request as Completed
                        await payload.ProjectRepository.UpdateBuildAndDeployRequest(r.Request.Id, Logger.ToString(), BuildAndDeployRequestState.Completed, null);

                        // Clear the Log.
                        Logger.Clear();
                    }
                }
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
            }

            Logger.Information($"{nameof(BuildAndDeployAction)} Done.");
        }

        public async Task DoWork(BuildAndDeployRequestModel request)
        {
            try
            {
                Logger.Information("Doing work 0..."); await Task.Delay(5_000);
                Logger.Information("Doing work 1..."); await Task.Delay(5_000);
                Logger.Information("Doing work 2..."); await Task.Delay(5_000);
                Logger.Information("Doing work 3..."); await Task.Delay(5_000);
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
            }
        }
    }

    public sealed class BuildAndDeployActionPayload : IRunnerActionPayload
    {
        public Guid ProjectId { get; set; }

        public IProjectRepository ProjectRepository { get; set; }

        public IServiceProvider ServiceProvider { get; set; }
    }
}