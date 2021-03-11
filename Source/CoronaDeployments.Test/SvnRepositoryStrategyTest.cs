using CoronaDeployments.Core;
using CoronaDeployments.Core.Models;
using CoronaDeployments.Core.RepositoryImporter;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CoronaDeployments.Test
{
    public class SvnRepositoryStrategyTest
    {
        [Fact]
        public async Task GetLastCommits()
        {
            var s = new SvnRepositoryStrategy();
            var p = new Project
            {
                Name = "TestProject",
                RepositoryUrl = "https://silverkey.repositoryhosting.com/svn/silverkey_silverkey_nrea",
            };
            var authInfo = new SvnAuthInfo(Email.Value2, Password.Value2);

            var config = new Core.AppConfiguration(@"C:\Repository\TestOldFashion");

            var result = await s.GetLastCommitsAsync(p, config, authInfo, 10);

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(10, result.Count);
        }
    }
}
