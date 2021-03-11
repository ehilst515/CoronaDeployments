using CoronaDeployments.Core.Models;
using System.Threading.Tasks;

namespace CoronaDeployments.Core
{
    public interface ISourceCodeBuilderStrategy
    {
        BuildTargetType Type { get; }

        Task<BuildStrategyResult> BuildAsync(BuildTarget target, string sourcePath, string outPath);
    }

    public record BuildStrategyResult(string Output, bool IsError);
}
