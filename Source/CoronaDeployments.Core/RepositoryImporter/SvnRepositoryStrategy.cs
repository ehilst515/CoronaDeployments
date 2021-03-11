using CoronaDeployments.Core.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CoronaDeployments.Core.RepositoryImporter
{
    public record BuildResult(BuildTarget Target, string OutputPath, bool HasErrors);

    public record DeployResult(BuildTarget Target, string Output, bool HasErrors);

    public sealed class SvnRepositoryStrategy : IRepositoryImportStrategy
    {
        public SourceCodeRepositoryType Type => SourceCodeRepositoryType.Svn;

        public Task<List<RepositoryCommit>> GetLastCommitsAsync(Project entity, AppConfiguration appConfiguration, IRepositoryAuthenticationInfo authInfo, int count)
        {
            return Task.Run(async () =>
            {
                var info = authInfo as AuthInfo;
                if (info == null)
                {
                    return null;
                }

                try
                {
                    var checkoutResult = await ImportAsync(entity, appConfiguration, authInfo);
                    if (checkoutResult.HasErrors)
                    {
                        return null;
                    }

                    List<RepositoryCommit> result = null;
                    using (var client = new SharpSvn.SvnClient())
                    {
                        //client.Authentication.DefaultCredentials = new NetworkCredential(info.Username, info.Password);

                        if (client.GetLog(checkoutResult.CheckOutDirectory, new SharpSvn.SvnLogArgs() { Limit = count }, out var items))
                        {
                            result = items
                                .Select(x => new RepositoryCommit(x.Revision.ToString(), x.LogMessage, x.Time.ToUniversalTime(), $"{x.Author}"))
                                .ToList();
                        }
                    }

                    try
                    {
                        // Clean up the directory.
                        Directory.Delete(checkoutResult.CheckOutDirectory, recursive: true);
                    }
                    catch (Exception exp)
                    {
                        Log.Error(exp, string.Empty);
                    }

                    return result;
                }
                catch (Exception exp)
                {
                    Log.Error(exp, string.Empty);

                    return null;
                }
            });
        }

        public Task<RepositoryImportResult> ImportAsync(Project project, AppConfiguration appConfiguration, IRepositoryAuthenticationInfo authInfo, string commitId = null)
        {
            return Task.Run(() =>
            {
                var folderName = $"{project.Name}_{DateTime.UtcNow.Ticks}";
                var path = Path.Combine(appConfiguration.BaseDirectory, folderName);

                var info = authInfo as AuthInfo;
                if (info == null)
                {
                    return new RepositoryImportResult(string.Empty, true);
                }

                try
                {
                    Directory.CreateDirectory(path);

                    using (var client = new SharpSvn.SvnClient())
                    {
                        client.Authentication.DefaultCredentials = new NetworkCredential(info.Username, info.Password);

                        Log.Information("Checking out...");

                        bool result = false;
                        
                        if (commitId == null)
                            result = client.CheckOut(new SharpSvn.SvnUriTarget(project.RepositoryUrl), path);
                        else
                            result = client.CheckOut(new SharpSvn.SvnUriTarget(project.RepositoryUrl), path, new SharpSvn.SvnCheckOutArgs
                            {
                                Revision = new SharpSvn.SvnRevision(int.Parse(commitId))
                            });

                        Log.Information($"Check out complete. Result = {result}");
                    }

                    Log.Information(string.Empty);

                    return new RepositoryImportResult(path, false);
                }
                catch (Exception exp)
                {
                    Log.Error(exp, string.Empty);

                    return new RepositoryImportResult(string.Empty, true);
                }
            });
        }
    }
}