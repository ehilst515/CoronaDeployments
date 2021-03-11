using CoronaDeployments.Core.Models;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoronaDeployments.Core
{
    public sealed class DotNetCoreSourceBuilderStrategy : ISourceCodeBuilderStrategy
    {
        public BuildTargetType Type => BuildTargetType.DotNetCore;

        public async Task<BuildStrategyResult> BuildAsync(BuildTarget target, string sourcePath, string outPath)
        {
            try
            {
                var cmd = $"dotnet publish {sourcePath} -c Release --self-contained -r win-x64 -o {outPath}";

                Log.Information(string.Empty);
                Log.Information(cmd);
                Log.Information(string.Empty);

                var output = await Shell.Execute(cmd);

                var isError = string.IsNullOrEmpty(output) || output.Contains(": error");

                return new BuildStrategyResult(output, isError);
            }
            catch (Exception exp)
            {
                Log.Error(exp, string.Empty);
                return new BuildStrategyResult(string.Empty, true);
            }
        }
    }
}
