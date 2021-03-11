using System.Threading.Tasks;

namespace CoronaDeployments.Core.RepositoryImporter
{
    public sealed record AuthInfo(string Username, string Password) : IRepositoryAuthenticationInfo
    {
        public async Task<bool> Validate()
        {
            if (string.IsNullOrWhiteSpace(Username)) return false;

            if (string.IsNullOrWhiteSpace(Password)) return false;

            return true;
        }
    }
}
