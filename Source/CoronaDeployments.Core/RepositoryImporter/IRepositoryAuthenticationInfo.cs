using System.Threading.Tasks;

namespace CoronaDeployments.Core.RepositoryImporter
{
    public interface IRepositoryAuthenticationInfo
    {
        Task<bool> Validate();
    }
}
