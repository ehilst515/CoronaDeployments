using CoronaDeployments.Core.Models;
using Microsoft.Web.Administration;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoronaDeployments.Core
{
    //public sealed class InternetInformationServerDeploymentStrategy : IDeployStrategy
    //{
    //    public DeployTargetType Type => DeployTargetType.IIS;

    //    public async Task<DeployStrategyResult> DeployAsync(BuildTarget target)
    //    {
    //        var info = target.DeploymentExtraInfo as IISDeployTargetExtraInfo;

    //        if (IISDeployTargetExtraInfo.Validate(info) == false)
    //        {
    //            return new DeployStrategyResult(string.Empty, true);
    //        }

    //        return await Task.Run(() =>
    //        {
    //            try
    //            {
    //                using (var manager = new ServerManager())
    //                {
    //                    var currentSites = manager.Sites;
    //                    foreach (var item in currentSites)
    //                    {
    //                        Log.Information(item.Name);
    //                    }

    //                    // Find out if the site exists already.
    //                    var site = manager.Sites.FirstOrDefault(x => x.Name == info.SiteName);
    //                    if (site == null)
    //                    {
    //                        site = manager.Sites.Add(info.SiteName, target.Result.OutputPath, info.Port);
    //                        manager.CommitChanges();
    //                    }
    //                    else
    //                    {
    //                        site.Stop();

    //                        site.Applications["/"].VirtualDirectories["/"].PhysicalPath = target.Result.OutputPath;

    //                        manager.CommitChanges();

    //                        site.Start();
    //                    }
    //                }

    //                return new DeployStrategyResult(string.Empty, false);
    //            }
    //            catch (Exception exp)
    //            {
    //                Log.Error(exp, string.Empty);
    //                return new DeployStrategyResult(string.Empty, true);
    //            }
    //        });
    //    }
    //}
}
