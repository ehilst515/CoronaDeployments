using CoronaDeployments.Core;
using CoronaDeployments.Core.Models.Mvc;
using CoronaDeployments.Core.Repositories;
using CoronaDeployments.Core.RepositoryImporter;
using CoronaDeployments.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoronaDeployments.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IProjectRepository projectRepo;
        private readonly IUserRepository userRepo;

        public HomeController(IProjectRepository repo, IUserRepository userRepo)
        {
            projectRepo = repo;
            this.userRepo = userRepo;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] LoginModel model, [FromServices] ISecurityRepository securityRepo)
        {
            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            var (error, session) = await securityRepo.Login(model.Username, model.Password);
            if (error != null)
            {
                this.AlertError(error);
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.SerialNumber, session.User.Id.ToString()),
                new Claim(ClaimTypes.Name, session.User.Name)
            };

            await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));

            await HttpContext.SetSession(session);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await projectRepo.GetAll();

            return View(result);
        }

        [HttpGet]
        public IActionResult CreateProject()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProject([FromForm] ProjectCreateModel model)
        {
            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            var result = await projectRepo.Create(model);
            if (result == false)
            {
                this.AlertError("Could not persist this object.");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult CreateBuildTarget([FromQuery] Guid projectId)
        {
            if (projectId == default)
                return BadRequest();

            return View(new BuildTargetCreateModel { ProjectId = projectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBuildTarget([FromForm] BuildTargetCreateModel model)
        {
            if (model.ProjectId == Guid.Empty)
                return BadRequest();

            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            var result = await projectRepo.CreateBuildTarget(model);
            if (result == false)
            {
                this.AlertError("Could not persist this object.");

                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View(new UserCreateModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser([FromForm] UserCreateModel model)
        {
            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            var result = await userRepo.Create(model);
            if (result == null)
            {
                this.AlertError("Could not persist this object.");

                return View(model);
            }

            this.AlertInfo($"User named: {model.Name} is created with password {model.GetPassword()}");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> CreateRepositoryCursor([FromQuery] Guid projectId,
            [FromServices] IEnumerable<IRepositoryImportStrategy> strategies,
            [FromServices] AppConfiguration appConfig,
            [FromServices] IRepositoryAuthenticationInfo authInfo)
        {
            var project = await projectRepo.Get(projectId);
            if (project == null)
            {
                return BadRequest();
            }

            var commits = await RepositoryManager.GetCommitList(project, project.RepositoryType,
                appConfig,
                authInfo,
                new ReadOnlyCollection<IRepositoryImportStrategy>(strategies.ToList()),
                10);

            var m = new RepositoryCursorCreateModel
            {
                ProjectId = projectId,
                ProjectName = project.Name,
                Commits = commits
            };

            return View(m);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRepositoryCursor([FromForm] RepositoryCursorCreateModel m)
        {
            //if (ModelState.IsValid == false)
            //{
            //    return View(model);
            //}

            var result = await projectRepo.CreateRepositoryCursor(m);
            if (result == false)
            {
                this.AlertError("Could not persist this object.");

                return View(m);
            }

            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}