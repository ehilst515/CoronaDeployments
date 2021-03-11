using CoronaDeployments.Core.Models;
using CoronaDeployments.Core.Models.Mvc;
using Marten;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoronaDeployments.Core.Repositories
{
    public interface IProjectRepository
    {
        public Task<bool> Create(ProjectCreateModel model);

        public Task<bool> CreateBuildTarget(BuildTargetCreateModel model);

        public Task<IReadOnlyList<Project>> GetAll();
        
        public Task<Project> Get(Guid id);
    }

    public class ProjectRepository : IProjectRepository
    {
        private IDocumentStore _store;

        public ProjectRepository(IDocumentStore store)
        {
            _store = store;
        }

        public async Task<bool> Create(ProjectCreateModel model)
        {
            using (var session = _store.OpenSession())
            {
                try
                {
                    var e = ToEntity(model);

                    session.Store(e);

                    await session.SaveChangesAsync();

                    return true;
                }
                catch (Exception exp)
                {
                    Log.Error(exp, string.Empty);
                    return false;
                }
            }
        }

        public async Task<bool> CreateBuildTarget(BuildTargetCreateModel model)
        {
            using (var session = _store.OpenSession())
            {
                try
                {
                    var e = ToEntity(model);

                    session.Store(e);

                    await session.SaveChangesAsync();

                    return true;
                }
                catch (Exception exp)
                {
                    Log.Error(exp, string.Empty);
                    return false;
                }
            }
        }

        public async Task<IReadOnlyList<Project>> GetAll()
        {
            using (var session = _store.OpenSession())
            {
                try
                {
                    var projects = await session.Query<Project>()
                        .ToListAsync();

                    // Batch in BuildTargets.
                    foreach (var p in projects)
                    {
                        var buildTargets = await session.Query<BuildTarget>()
                            .Where(x => x.ProjectId == p.Id)
                            .ToListAsync();

                        p.BuildTargets = buildTargets;
                    }

                    return projects;
                }
                catch (Exception exp)
                {
                    Log.Error(exp, string.Empty);
                    return null;
                }
            }
        }

        public async Task<Project> Get(Guid id)
        {
            using (var session = _store.OpenSession())
            {
                try
                {
                    var project = await session.Query<Project>()
                        .FirstOrDefaultAsync(x => x.Id == id);

                    return project;
                }
                catch (Exception exp)
                {
                    Log.Error(exp, string.Empty);
                    return null;
                }
            }
        }

        public Project ToEntity(ProjectCreateModel m)
        {
            return new Project
            {
                Name = m.Name,
                BranchName = m.BranchName,
                RepositoryType = m.RepositoryType,
                RepositoryUrl = m.RepositoryUrl,
                CreatedAtUtc = DateTime.UtcNow
            };
        }

        public BuildTarget ToEntity(BuildTargetCreateModel m)
        {
            return new BuildTarget
            {
                Name = m.Name,
                Type = m.Type,
                DeploymentType = m.DeploymentType,
                TargetRelativePath = m.TargetRelativePath,
                ProjectId = m.ProjectId,
                CreatedAtUtc = DateTime.UtcNow
            };
        }
    }
}