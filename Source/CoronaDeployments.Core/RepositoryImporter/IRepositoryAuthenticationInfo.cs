using System.Threading.Tasks;

namespace CoronaDeployments.Core
{
    public interface IRepositoryAuthenticationInfo
    {
        Task<bool> Validate();
    }
}
