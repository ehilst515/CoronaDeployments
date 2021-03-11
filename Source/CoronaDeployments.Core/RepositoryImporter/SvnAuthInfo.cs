using System.Threading.Tasks;

namespace CoronaDeployments.Core
{
    public sealed record SvnAuthInfo(string Username, string Password) : IRepositoryAuthenticationInfo
    {
        public async Task<bool> Validate()
        {
            if (string.IsNullOrWhiteSpace(Username)) return false;

            if (string.IsNullOrWhiteSpace(Password)) return false;

            return true;
        }
    }
}
