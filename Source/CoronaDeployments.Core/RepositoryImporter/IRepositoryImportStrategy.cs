using CoronaDeployments.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoronaDeployments.Core.RepositoryImporter
{
    public interface IRepositoryImportStrategy
    {
        SourceCodeRepositoryType Type { get; }

        Task<RepositoryImportResult> ImportAsync(Project entity, AppConfiguration appConfiguration, IRepositoryAuthenticationInfo authInfo, string commitId = null);

        Task<List<RepositoryCommit>> GetLastCommitsAsync(Project entity, AppConfiguration appConfiguration, IRepositoryAuthenticationInfo authInfo, int count);
    }

    public record RepositoryImportResult(string CheckOutDirectory, bool HasErrors);

    public record RepositoryCommit(string CommitId, string CommitComment, DateTime CommitStamp, string CommitExtra);
}
