using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoronaDeployments.Core.RepositoryImporter
{
    public sealed class GitAuthInfo : IRepositoryAuthenticationInfo
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public async Task<bool> Validate()
        {
            return true;
        }
    }
}
