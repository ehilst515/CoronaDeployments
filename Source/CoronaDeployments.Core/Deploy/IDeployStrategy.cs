using CoronaDeployments.Core.Models;
using System.Threading.Tasks;

namespace CoronaDeployments.Core
{
    public interface IDeployStrategy
    {
        DeployTargetType Type { get; }

        Task<DeployStrategyResult> DeployAsync(BuildTarget target);
    }
}
