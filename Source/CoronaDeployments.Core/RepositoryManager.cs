using CoronaDeployments.Core.Models;
using CoronaDeployments.Core.RepositoryImporter;
using Serilog;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoronaDeployments.Core
{
    public static class RepositoryManager
    {
        public static async Task<RepositoryImportResult> ImportAsync(Project project, SourceCodeRepositoryType repoType, AppConfiguration appConfiguration, 
            IRepositoryAuthenticationInfo authInfo,
            ReadOnlyCollection<IRepositoryImportStrategy> strategies)
        {
            if (authInfo != null)
            {
                if (await authInfo.Validate() == false)
                {
                    Log.Information($"Validation for {nameof(authInfo)} did not pass.");

                    return new RepositoryImportResult(string.Empty, true);
                }
            }

            var strategy = strategies.FirstOrDefault(x => x.Type == repoType);

            if (strategy == null)
            {
                Log.Error($"Unknown Source Code Import type: {repoType}");
                return default;
            }

            var result = await strategy.ImportAsync(project, appConfiguration, authInfo);

            return result;
        }

        public static async Task<List<RepositoryCommit>> GetCommitList(Project p, SourceCodeRepositoryType repoType, AppConfiguration appConfiguration, 
            IRepositoryAuthenticationInfo authInfo,
            ReadOnlyCollection<IRepositoryImportStrategy> strategies, 
            int count)
        {
            if (authInfo != null)
            {
                if (await authInfo.Validate() == false)
                {
                    Log.Information($"Validation for {nameof(authInfo)} did not pass.");

                    return null;
                }
            }

            var strategy = strategies.FirstOrDefault(x => x.Type == repoType);

            if (strategy == null)
            {
                Log.Error($"Unknown Source Code Import type: {repoType}");
                return null;
            }

            var result = await strategy.GetLastCommitsAsync(p, appConfiguration, authInfo, count);

            return result;
        }
    }
}
