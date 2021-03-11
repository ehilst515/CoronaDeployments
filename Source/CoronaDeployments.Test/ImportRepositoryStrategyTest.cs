using CoronaDeployments.Core.Models;
using CoronaDeployments.Core.RepositoryImporter;
using System.Threading.Tasks;
using Xunit;

namespace CoronaDeployments.Test
{
    public class ImportRepositoryStrategyTest
    {
        [Fact]
        public async Task GitRepositoryStrategy()
        {
            var s = new GitRepositoryStrategy();
            var p = new Project
            {
                Name = "TestProject",
                RepositoryUrl = "https://github.com/SherifRefaat/CoronaDeployments.git",
                BranchName = "main",
            };

            var result = await s.ImportAsync(
                p,
                new Core.AppConfiguration(@"C:\Repository\TestOldFashion"), 
                new GitAuthInfo
                {
                    Username = Email.Value1,
                    Password = Password.Value1
                });

            Assert.False(result.HasErrors);
        }
    }
}