using System.Threading.Tasks;

namespace CoronaDeployments.Core.Runner
{
    public interface IRunnerAction
    {
        Task Implementation(IRunnerActionPayload payload);
    }

    public interface IRunnerActionPayload
    {
    }
}
