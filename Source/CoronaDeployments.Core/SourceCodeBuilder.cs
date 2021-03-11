using CoronaDeployments.Core.Models;
using Serilog;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoronaDeployments.Core
{
    public static class SourceCodeBuilder
    {
        public static async Task<ReadOnlyCollection<BuildResult>> BuildTargetsAsync(string checkOutDirectory, BuildTarget[] targets, ReadOnlyCollection<ISourceCodeBuilderStrategy> strategies)
        {
            var result = new List<BuildResult>();
            foreach (var t in targets)
            {
                var sourcePath = Path.Combine(checkOutDirectory, t.TargetRelativePath);
                var outPath = Path.Combine(checkOutDirectory, t.Name);

                Log.Information($"Building Target: {t.Type} {t.Name} {t.TargetRelativePath}");

                BuildStrategyResult currentResult = default;
                var strategy = strategies.FirstOrDefault(x => x.Type == t.Type);

                if (strategy == null)
                {
                    Log.Error($"Unknown build target type: {t.Type}");
                    continue;
                }

                currentResult = await strategy.BuildAsync(t, sourcePath, outPath);

                Log.Information($"Output: IsError: {currentResult.IsError}");
                Log.Information(currentResult.Output);
                Log.Information(string.Empty);

                result.Add(new BuildResult(t, outPath, currentResult.IsError));
            }

            return result.AsReadOnly();
        }
    }
}
