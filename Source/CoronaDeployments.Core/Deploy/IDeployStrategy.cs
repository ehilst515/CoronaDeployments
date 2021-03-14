using CoronaDeployments.Core.Models;
using CoronaDeployments.Core.Runner;
using System.Threading.Tasks;

namespace CoronaDeployments.Core
{
    public interface IDeployStrategy
    {
        DeployTargetType Type { get; }

        Task<DeployStrategyResult> DeployAsync(BuildTarget target, CustomLogger customLogger);
    }
}
