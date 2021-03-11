using CoronaDeployments.Core.Models;
using Serilog;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoronaDeployments.Core
{
    public static class RepositoryManager
    {
        public static async Task<RepositoryImportResult> ImportAsync(Project project, SourceCodeRepositoryType repoType, AppConfiguration appConfiguration, IRepositoryAuthenticationInfo authInfo,
            ReadOnlyCollection<IRepositoryImportStrategy> strategies)
        {
            if (authInfo != null)
            {
                if (await authInfo.Validate())
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

        public static async Task GetCommitList(Project p, SourceCodeRepositoryType repoType, IRepositoryAuthenticationInfo authInfo,
            ReadOnlyCollection<IRepositoryImportStrategy> strategies)
        {
            if (authInfo != null)
            {
                if (await authInfo.Validate())
                {
                    Log.Information($"Validation for {nameof(authInfo)} did not pass.");

                    return;
                }
            }

            var strategy = strategies.FirstOrDefault(x => x.Type == repoType);

            if (strategy == null)
            {
                Log.Error($"Unknown Source Code Import type: {repoType}");
                return;
            }

            return;
        }
    }
}
