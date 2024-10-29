using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PgManagerApp.Models;
using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;
using System.Text.Json;

namespace PgManagerApp.Controllers
{
    /*    [Authorize]*/
    [Authorize(Roles = "User, Admin")]
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
            string? userId = HttpContext.Request.Cookies["UserId"];
            bool isEmployee = false;
            isEmployee = userId.ToUpper().Contains("EMP");

            var emp = _context.Users.Where(x => x.UserId == userId).SingleOrDefault();
            var dateNow = DateTime.Now;
            var checkInOutData = isEmployee ? _context.CheckInOuts.Where(x => x.EmpNameWithDes.ToLower().Contains(emp.UserId.ToLower()) && x.Date.Date == dateNow.Date).SingleOrDefault() : null;

            DashboardViewModel model = new DashboardViewModel
            {
                UserId = userId,
                TotalProjects = _context.Projects.ToList().Count().ToString(),
                TotalTasks = isEmployee ? _context.TaskDetails.Where(x => x.AssignedToId == userId).ToList().Count().ToString() : _context.TaskDetails.ToList().Count().ToString(),
                TotalUsers = _context.Users.ToList().Count().ToString(),
                TotalHolidays = _context.CalendarEvents.Where(x => x.EventName.ToLower().Contains("holiday")).ToList().Count().ToString(),
                IsEmployee = isEmployee,
                Users = _context.Users.ToList(),
                IsCheckedIn = (checkInOutData != null) ? true : false
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

        public IActionResult AddDesignation(string designationName)
        {
            if (designationName != null)
            {
                Designation des = new Designation { Name = designationName };
                _context.Designations.Add(des);
                _context.SaveChanges();
                TempData["Success"] = "Designation added succesfully";
            }
            else
            {
                TempData["Error"] = "Something went wrong!";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult SaveTimer([FromBody] UserTimer userTimer)
        {
            if (userTimer == null || userTimer.StartTime == DateTime.MinValue || userTimer.EndTime == DateTime.MinValue)
            {
                return BadRequest("Invalid timer data.");
            }

            _context.UserTimers.Add(userTimer);
            _context.SaveChanges();

            return Ok(); // Return some response if needed
        }

        [HttpGet]
        public IActionResult GetNotifications()
        {
            string? userId = HttpContext.Request.Cookies["UserId"];
            bool isEmployee = false;
            isEmployee = userId.ToUpper().Contains("EMP");

            var taskDetails = _context.TaskDetails.Where(x => x.Status.ToLower().Contains("todo") || x.Status.ToLower().Contains("inprogress")).ToList();

            string count = isEmployee ? taskDetails.Where(x => x.AssignedToId == userId).ToList().Count().ToString() : "0";
            return Json(count);
        }
        [HttpGet]
        public IActionResult GetPreviousNotificationCount()
        {
            // Retrieve the count from session
            var previousCount = HttpContext.Session.GetInt32("PreviousNotificationCount") ?? 0;
            return Ok(previousCount);
        }

        [HttpPost]
        public IActionResult SetPreviousNotificationCount([FromBody] NotificationCountModel model)
        {
            // Store the count in session
            HttpContext.Session.SetInt32("PreviousNotificationCount", model.Count);
            return Ok();
        }
        public class NotificationCountModel
        {
            public int Count { get; set; }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
