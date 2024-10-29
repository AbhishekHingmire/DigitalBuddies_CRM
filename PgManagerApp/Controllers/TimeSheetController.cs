using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PgManagerApp.Models;

namespace PgManagerApp.Controllers
{
    [Authorize(Roles = "User")]
    public class TimeSheetController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TimeSheetController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            string? userId = HttpContext.Request.Cookies["UserId"];
            bool isEmployee = false;
            isEmployee = userId.ToUpper().Contains("EMP");
            var emp = _context.Users.Where(x => x.UserId == userId).SingleOrDefault();

            var timesheet = new TimeShetViewModel();
            timesheet.IsEmployee = isEmployee;
            timesheet.UserTimers = isEmployee ? _context.UserTimers.Where(x => x.UserId == userId).ToList() : _context.UserTimers.ToList();
            timesheet.UserDetails = isEmployee ? _context.Users.Where(x => x.UserId == userId).ToList() : _context.Users.ToList();
            return View(timesheet);
        }
    }
}
