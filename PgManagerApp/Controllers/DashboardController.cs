using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PgManagerApp.Models;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;

namespace PgManagerApp.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly ApplicationDbContext _context;

        public DashboardController(ILogger<DashboardController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            DashboardViewModel model = new DashboardViewModel
            {
                HolidayList = "1",
                TotalProjects = "50",
                TotalTasks = "17",
                TotalUsers = "20"
            };
            return View(model);
        }
        public IActionResult AddProject(string projectName)
        {
            if (projectName != null)
            {
                Project proj = new Project { Name = projectName };
                _context.Projects.Add(proj);
                _context.SaveChanges();
                TempData["Success"] = "Project added succesfully";
            }
            else
            {
                TempData["Error"] = "Something went wrong!";
            }
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
