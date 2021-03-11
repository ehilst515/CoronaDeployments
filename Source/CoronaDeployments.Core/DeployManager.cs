using CoronaDeployments.Core.Models;
using Serilog;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CoronaDeployments.Core
{
    public static class DeployManager
    {
        public static async Task<ReadOnlyCollection<DeployResult>> DeployTargetsAsync(BuildTarget[] targets, IReadOnlyCollection<IDeployStrategy> strategies)
        {
            var result = new List<DeployResult>();
            foreach (var t in targets)
            {
                Log.Information($"Deploying Target: {t.Type} {t.Name} {t.TargetRelativePath}");

                DeployStrategyResult currentResult = default;
                var strategy = strategies.FirstOrDefault(x => x.Type == t.DeploymentType);

                if (strategy == null)
                {
                    Log.Error($"Unknown deploy target type: {t.Type}");
                    continue;
                }

                currentResult = await strategy.DeployAsync(t);

                Log.Information($"Output: IsError: {currentResult.IsError}");
                Log.Information(currentResult.Output);
                Log.Information(string.Empty);

                result.Add(new DeployResult(t, currentResult.Output, currentResult.IsError));
            }

            return result.AsReadOnly();
        }
    }

    public record DeployStrategyResult(string Output, bool IsError);
}
